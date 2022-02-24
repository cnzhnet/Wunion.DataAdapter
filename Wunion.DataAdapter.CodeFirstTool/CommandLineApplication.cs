using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;
using TeleprompterConsole.ProjectAnalysis;
using TeleprompterConsole.Generating;

namespace TeleprompterConsole
{
    /// <summary>
    /// 命令行应用程序.
    /// </summary>
    internal class CommandLineApplication
    {
        private CommandLineApplication(ILanguageProvider lang)
        {
            Language = lang;
        }

        /// <summary>
        /// 获取或设置语言环境.
        /// </summary>
        internal ILanguageProvider Language { get; private set; }

        /// <summary>
        /// 表示目标数据类型.
        /// </summary>
        internal string DbKind { get; private set; }

        /// <summary>
        /// 运行指定的命令.
        /// </summary>
        /// <param name="args"></param>
        internal void Run(string[] args)
        {
            try
            {
#if DEBUG
                string currentDir = @"D:\cnzhnet\Documents\Visual Studio 2019\Projects\Wunion.DataAdapter.NetCore\Wunion.DataAdapter.CodeFirstDemo";
                
#else
                string currentDir = Directory.GetCurrentDirectory();
#endif
                // 根据命令行数初始化目标数据库引擎
                DataEngine dbEngine = GetDatabaseEngine(args);
                Console.WriteLine(Language.GetString("analyzing_target_project"));
                ProjectAnalyzer projAnalyzer = new ProjectAnalyzer(currentDir, Language);
                projAnalyzer.Run();
                ProjectInfo pi = projAnalyzer.Project;
                Console.WriteLine(pi.ToString());
                // 分析目标程序集.
                string assemblyName = $"{projAnalyzer.Project.AssemblyName}.dll";
                Console.WriteLine(Language.GetString("analyzing_target_dll", assemblyName));
                AssemblyCodeAnalyzer codeAnalyzer = CodeAnalyzer.LoadAssembly(assemblyName, pi.OutputPath, Language);
                codeAnalyzer.WriteLog = (message) => Console.WriteLine(message);
                codeAnalyzer.DbEngine = dbEngine;
                using (codeAnalyzer)
                {
                    codeAnalyzer.Run(DbKind);
                    DatabaseGenerator generator = new DatabaseGenerator(currentDir, Language);
                    generator.TargetProject = pi;
                    generator.TargetAssembly = codeAnalyzer.TargetAssembly;
                    generator.WriteLog = (message) => Console.WriteLine(message);

                    foreach (DbContextDeclaration declaration in codeAnalyzer.DbContexts)
                        generator.Run(declaration);
                }
                Console.WriteLine(Language.GetString("code_first_completed"));
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
            }
#if DEBUG
            Console.ReadKey();
#endif
        }

        /// <summary>
        /// 从命令行中参数获取数据库类型.
        /// </summary>
        /// <param name="args">命令行中参数</param>
        /// <returns></returns>
        private DataEngine GetDatabaseEngine(string[] args)
        {
            List<string> supportedDb = new List<string>(new string[] {
                "mssql", "mysql", "npgsql", "sqlite3"
            });
            string kind = args.FirstOrDefault(p => supportedDb.Count(it => p.ToLower().StartsWith(it)) > 0);
            if (string.IsNullOrEmpty(kind))
                throw new Exception(Language.GetString("database_kind_missing"));

            DataEngine dbEngine = null;
            if (kind.ToLower().StartsWith("mysql")) // MySQL 数据库引擎的配置.
            {
                string engine = Wunion.DataAdapter.Kernel.MySQL.StorageEngine.INNODB;
                string[] array = kind.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length > 1)
                    engine = array[1];
                dbEngine = new DataEngine(
                    new Wunion.DataAdapter.Kernel.MySQL.MySqlDBAccess(),
                    new Wunion.DataAdapter.Kernel.MySQL.CommandParser.MySqlParserAdapter(engine)
                );
                DbKind = array.First().ToLower();
            }
            else // 其它数据库的配置.
            {
                DbKind = kind.ToLower();
                switch (DbKind)
                {
                    case "mssql":
                        dbEngine = new DataEngine(
                            new Wunion.DataAdapter.Kernel.SQLServer.SqlServerDbAccess(),
                            new Wunion.DataAdapter.Kernel.SQLServer.CommandParser.SqlServerParserAdapter()
                        );
                        break;
                    case "npgsql":
                        dbEngine = new DataEngine(
                            new Wunion.DataAdapter.Kernel.PostgreSQL.NpgsqlDbAccess(),
                            new Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser.NpgsqlParserAdapter()
                        );
                        break;
                    case "sqlite3":
                        dbEngine = new DataEngine(
                            new Wunion.DataAdapter.Kernel.SQLite3.SqliteDbAccess(),
                            new Wunion.DataAdapter.Kernel.SQLite3.CommandParser.SqliteParserAdapter()
                        );
                        break;
                }
            }
            if (dbEngine == null)
                throw new NotSupportedException(Language.GetString("database_not_supported", kind));
            return dbEngine;
        }

        /// <summary>
        /// 创建一个 <see cref="CommandLineApplication"/>的对象实例.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        internal static CommandLineApplication Create(ILanguageProvider lang)
        {
            return new CommandLineApplication(lang);
        }
    }
}
