using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CodeFirst;

namespace TeleprompterConsole.ProjectAnalysis
{
    /// <summary>
    /// 表示目标程序集的代码分析器.
    /// </summary>
    public class AssemblyCodeAnalyzer : CodeAnalyzer
    {
        private string dotnetRoot;
        private AssemblyLoadContext context;
        private string[] packPaths;
        private string dbKind;

        /// <summary>
        /// 创建一个 <see cref="AssemblyCodeAnalyzer"/> 的对象实例.
        /// </summary>
        /// <param name="assemblyName">目标程序集的名称.</param>
        /// <param name="targetDir">目标程序集所在的目标（绝对路径）.</param>
        /// <param name="lang">语言环境.</param>
        internal AssemblyCodeAnalyzer(string assemblyName, string targetDir, ILanguageProvider lang)
        {
            Langauge = lang;
            // 获取dotnet的根目录.
            const string dotnet = "dotnet";
            string dotnetVersion = Environment.Version.ToString();
            dotnetRoot = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            int index = dotnetRoot.IndexOf(dotnet);
            dotnetRoot = dotnetRoot.Substring(0, index + dotnet.Length);
            packPaths = new string[2] {
                Path.Combine(dotnetRoot, "shared", "Microsoft.NETCore.App", dotnetVersion),
                Path.Combine(dotnetRoot, "shared", "Microsoft.AspNetCore.App", dotnetVersion)
            };
            List<string> loadedDll = new List<string>(new string[] {
                "Wunion.DataAdapter.NetCore.dll",
                "Wunion.DataAdapter.NetCore.MySQL.dll",
                "Wunion.DataAdapter.NetCore.PostgreSQL.dll",
                "Wunion.DataAdapter.NetCore.SQLite3.dll",
                "Wunion.DataAdapter.NetCore.SQLServer.dll"
            });

            context = new AssemblyLoadContext("Target_Library", true);
            // 加载目标程序集及其所有信赖项.
            string[] files = Directory.GetFiles(targetDir, "*.dll");
            string dllFile;
            Assembly assem;
            foreach (string s in files)
            {
                dllFile = Path.GetFileName(s);
                if (loadedDll.Contains(dllFile))
                    continue;
                assem = context.LoadFromAssemblyPath(s);
                if (dllFile == assemblyName)
                    TargetAssembly = assem;
            }
            // 加载其它可能漏掉的框架依赖项.
            AssemblyName[] assemblies = TargetAssembly.GetReferencedAssemblies();
            string assemblyPath;
            int i;
            foreach (AssemblyName assemName in assemblies)
            {
                if (AppDomain.CurrentDomain.GetAssemblies().Count(p => p.GetName().Name == assemName.Name) > 0)
                    continue;
                if (context.Assemblies.Count(p => p.GetName().Name == assemName.Name) > 0)
                    continue;
                for (i = 0; i < packPaths.Length; ++i)
                {
                    assemblyPath = Path.Combine(packPaths[i], $"{assemName.Name}.dll");
                    if (!File.Exists(assemblyPath))
                        continue;
                    context.LoadFromAssemblyPath(assemblyPath);
                }
            }
        }

        /// <summary>
        /// 目标程序集.
        /// </summary>
        public Assembly TargetAssembly { get; private set; }

        /// <summary>
        /// 表示目标程序集中的数据库架构定义的分析结果.
        /// </summary>
        internal DbContextDeclaration[] DbContexts { get; private set; }

        /// <summary>
        /// 运行代码分析.
        /// </summary>
        /// <param name="arg">无用参数.</param>
        public override void Run(object arg = null)
        {
            DbContexts = null;
            dbKind = arg?.ToString();
            // 获取此程序集中的所有类型.
            Type[] allTypes = TargetAssembly.GetTypes();
            Type baseType;
            List<Type> dbTypes = new List<Type>();
            foreach (Type t in allTypes)
            {
                baseType = t.BaseType;
                if (baseType == null)
                    continue;
                // 父类为 DbContext 类则可确定该类型为数据库的上下文定义
                if (baseType == typeof(DbContext))
                    dbTypes.Add(t);
            }
            // 分析目标项目的数据库定义
            List<DbContextDeclaration> schemas = new List<DbContextDeclaration>();
            foreach (Type dbt in dbTypes)
                DbContextAnalysis(dbt, schemas);
            DbContexts = schemas.ToArray();
        }

        /// <summary>
        /// 分析数据库定义.
        /// </summary>
        /// <param name="dbcType">定义的数据库上下文对象类型.</param>
        /// <param name="output">用于输出数据据库架构的分析结果.</param>
        private void DbContextAnalysis(Type dbcType, List<DbContextDeclaration> output)
        {
            WriteLog?.Invoke(Langauge.GetString("database_context_found", dbcType.Name));
            PropertyInfo[] properties = dbcType.PublicInstanceProperties();
            DbContext dbContext = CreateDbContext(dbcType);
            GeneratingOptions options = new GeneratingOptions(new DbGenerateOption(dbKind));
            dbContext.OnBeforeGenerating(options); // 调用目标项目中实现的生成配置.
            options.ThrowExceptionIfInvalid(Langauge, dbcType);
            DbContextDeclaration dbSchema = new DbContextDeclaration { 
                DBC = dbContext,
                TargetAssembly = TargetAssembly,
                Generating = (GeneratingOptions)options
            };
            // 分析目标 DbContext 的表定义.
            List<DbTableDeclaration> tablesCache = new List<DbTableDeclaration>();
            IDbTableContext tableContext;
            Type entityType;
            DbTableDeclaration tableDeclaration;
            GenerateOrderAttribute generateOrder;
            foreach (PropertyInfo pi in properties)
            {
                tableContext = pi.GetValue(dbContext) as IDbTableContext;
                if (tableContext == null)
                    continue;
                entityType = pi.PropertyType.GenericTypeArguments.First();
                tableDeclaration = new DbTableDeclaration(tableContext.tableName, entityType);
                generateOrder = pi.GetCustomAttribute<GenerateOrderAttribute>();
                if (generateOrder != null)
                    tableDeclaration.Order = generateOrder.Index;
                tablesCache.Add(tableDeclaration);
            }
            dbSchema.TableDeclarations = (from p in tablesCache orderby p.Order ascending select p).ToArray();
            output.Add(dbSchema);
        }

        /// <summary>
        /// 创建数据库上下文对象实例.
        /// </summary>
        /// <param name="dbcType">数据库上下文对象的类型.</param>
        /// <returns></returns>
        private DbContext CreateDbContext(Type dbcType)
        {
            DbContext dbContext = null;
            ConstructorInfo method = dbcType.GetConstructor(new Type[] { typeof(DataEngine) });
            if (method != null)
            {
                dbContext = method.Invoke(new object[] { DbEngine }) as DbContext;
            }
            return dbContext;
        }

        /// <summary>
        /// 释放对象所占用的资源.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            context?.Unload();
            if (disposing)
            {
                context = null;
            }
        }
    }
}
