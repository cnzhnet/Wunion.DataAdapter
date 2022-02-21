using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Wunion.DataAdapter.Kernel.CodeFirst;
using TeleprompterConsole.ProjectAnalysis;

namespace TeleprompterConsole.Generating
{
    /// <summary>
    /// 用于生成实体查询的数据访问器类.
    /// </summary>
    public class DaoCodeGenerator : Generator
    {
        private string codesPath;
        private XDocument comments;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projRoot"></param>
        /// <param name="lang"></param>
        internal DaoCodeGenerator(string projRoot, ILanguageProvider lang) : base(projRoot, lang)
        { }

        /// <summary>
        /// 加载目标项目的注释信息.
        /// </summary>
        private void LoadComments()
        {
            if (!File.Exists(TargetProject.CommentsFile))
            {
                comments = null;
                return;
            }
            comments = XDocument.Load(TargetProject.CommentsFile);
        }

        /// <summary>
        /// 将指定的缩进占位.
        /// </summary>
        /// <param name="level">缩进级别.</param>
        /// <returns></returns>
        private string GetIndent(int level = 0)
        {
            if (level < 1)
                return string.Empty;
            int spaceCount = level * 4;
            StringBuilder sb = new StringBuilder();
            while (spaceCount > 0)
            {
                sb.Append(" ");
                spaceCount--;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 设置类的注释.
        /// </summary>
        /// <param name="writer">代码写入器.</param>
        /// <param name="entity">实体类型.</param>
        /// <param name="pattern">值为 T 表示类注释；值为 P 表示属性注释；值为 M 表示方法注释；</param>
        /// <param name="elem">注释的节点名称.</param>
        private string GetComment(TextWriter writer, Type entity, char pattern, string member = null, string elem = "summary")
        {
            if (comments == null)
                return string.Empty;
            string xname = null;
            switch (pattern)
            {
                case 'T':
                    xname = $"T:{entity.FullName}";
                    break;
                case 'F':
                    xname = $"F:{entity.FullName}.{member}";
                    break;
                case 'P':
                    xname = $"P:{entity.FullName}.{member}";
                    break;
                case 'M':
                    xname = $"M:{entity.FullName}.{member}";
                    break;
                default:
                    return string.Empty;
            }
            XElement target = comments.Root.Element("members").Elements("member").Where(p => p.Attribute("name").Value == xname).FirstOrDefault();
            if (target == null)
                return string.Empty;
            target = target.Element(elem);
            if (target == null)
                return string .Empty;
            return target.Value.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
        }

        /// <summary>
        /// 引入文件的命名空间.
        /// </summary>
        /// <param name="writer"></param>
        private void ImportNamespaces(TextWriter writer, string declreNamespace)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using Wunion.DataAdapter.Kernel.CommandBuilders;");
            writer.WriteLine("using Wunion.DataAdapter.Kernel.CodeFirst;");
            writer.WriteLine("using Wunion.DataAdapter.Kernel.Querying;");
            writer.WriteLine("\r\n");
            writer.WriteLine($"namespace {declreNamespace}");
            writer.WriteLine("{");
        }

        /// <summary>
        /// 定义属性.
        /// </summary>
        /// <param name="writer">代码写入器.</param>
        /// <param name="entity">实体类型.</param>
        /// <param name="pi">实体的属性.</param>
        /// <param name="attribute">实体属性的字段映射描述.</param>
        private void DeclareFieldProperty(TextWriter writer, Type entity, PropertyInfo pi, TableFieldAttribute attribute)
        {
            writer.Write("\r\n");
            string indent = GetIndent(2);
            string comment = GetComment(writer, entity, 'P', member: pi.Name);
            if (!string.IsNullOrEmpty(comment)) // 编写注释
            {
                writer.WriteLine($"{indent}/// <summary>");
                writer.WriteLine($"{indent}/// {comment}");
                writer.WriteLine($"{indent}/// </summary>");
            }
            if (string.IsNullOrEmpty(attribute.Name))
                attribute.Name = pi.Name;
            writer.WriteLine($"{indent}public FieldDescription {pi.Name} => GetField(\"{attribute.Name}\");");
        }

        /// <summary>
        /// 实现 DAO 类的 GetTableContext 方法.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="table"></param>
        private void DeclareGetTableContext(TextWriter writer, DbTableDeclaration table)
        {
            writer.Write("\r\n");
            string indent = GetIndent(2);
            writer.WriteLine($"{indent}protected override IDbTableContext GetTableContext(string name)");
            writer.Write($"{indent}");
            writer.Write("{");
            writer.WriteLine($"\r\n{GetIndent(3)}if (string.IsNullOrEmpty(name))");
            writer.WriteLine($"{GetIndent(4)}return db?.TableDeclaration<{table.EntityType.Name}>(\"{table.Name}\");");
            writer.WriteLine($"{GetIndent(3)}return db?.TableDeclaration<{table.EntityType.Name}>(name);");
            writer.Write($"{GetIndent(2)}");
            writer.WriteLine("}");
        }

        /// <summary>
        /// 实现 DAO 类的 GetAll{T} 方法.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="fields"></param>
        private void DeclareGetAll(TextWriter writer, PropertyInfo[] fields)
        {
            writer.Write("\r\n");
            string indent = GetIndent(2);
            writer.WriteLine($"{indent}protected override IDescription[] GetAllFields()");
            writer.Write($"{indent}");
            writer.Write("{\r\n");
            indent = GetIndent(3);
            writer.Write($"{indent}return new IDescription[]");
            writer.Write(" {");
            indent = GetIndent(4);
            writer.Write($"\r\n{indent}{fields.First().Name}");
            for (int i = 1; i < fields.Length; ++i)
                writer.Write($",\r\n{indent}{fields[i].Name}");
            indent = GetIndent(3);
            writer.Write($"\r\n{indent}");
            writer.WriteLine("};");
            writer.Write($"{GetIndent(2)}");
            writer.Write("}");
        }

        /// <summary>
        /// 定义指定实体对应的 DAO 类.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="table">表的定义.</param>
        /// <param name="writer"></param>
        private void DeclareClass(string typeName, DbTableDeclaration table, TextWriter writer)
        {
            string indent = GetIndent(1);
            string comment = GetComment(writer, table.EntityType, 'T');
            if (!string.IsNullOrEmpty(comment)) // 编写注释
            {
                writer.WriteLine($"{indent}/// <summary>");
                writer.WriteLine($"{indent}/// {comment} DAO .");
                writer.WriteLine($"{indent}/// </summary>");
            }
            writer.WriteLine($"{indent}public class {typeName} : QueryDao");
            writer.Write($"{indent}");
            writer.Write("{\r\n");

            // 编写构造函数.
            indent = GetIndent(2);
            writer.WriteLine($"{indent}public {typeName}(DbContext dbc = null) : base(dbc)");
            writer.Write($"{indent}");
            writer.Write("{ }\r\n");
            writer.WriteLine($"\r\n{indent}public {typeName}() : base(null)");
            writer.Write($"{indent}");
            writer.Write("{ }\r\n");

            writer.WriteLine($"\r\n{indent}public override Type EntityType => typeof({table.EntityType.Name});");

            // 根据实体类的属性创建 DAO 查询访问类的属性.
            PropertyInfo[] properties = table.EntityType.PublicInstanceProperties();
            TableFieldAttribute fieldAttr;
            foreach (PropertyInfo pi in properties)
            {
                fieldAttr = pi.GetCustomAttribute<TableFieldAttribute>();
                if (fieldAttr == null)
                    continue;
                DeclareFieldProperty(writer, table.EntityType, pi, fieldAttr);
            }
            DeclareGetTableContext(writer, table);
            DeclareGetAll(writer, properties);

            indent = GetIndent(1);
            writer.Write($"\r\n{indent}");
            writer.Write("}");
        }

        /// <summary>
        /// 运行生成任务.
        /// </summary>
        /// <param name="arg"></param>
        public override void Run(DbContextDeclaration arg)
        {
            WriteLog?.Invoke("-------------------------------------");
            WriteLog?.Invoke(Language.GetString("dao_code_generating"));
            codesPath = Path.Combine(ProjectRoot, arg.Generating.DaoGenerateDirectory);
            if (!Directory.Exists(codesPath))
                Directory.CreateDirectory(codesPath);
            LoadComments();
            TextWriter writer;
            string typeName;
            foreach (DbTableDeclaration table in arg.TableDeclarations)
            {
                typeName = $"{table.EntityType.Name}Dao";
                using (writer = new StreamWriter(Path.Combine(codesPath, $"{typeName}.cs"), false, Encoding.UTF8))
                {
                    ImportNamespaces(writer, arg.Generating.DaoGenerateNamespace);
                    DeclareClass(typeName, table, writer);
                    writer.WriteLine("\r\n}"); // 命名空间的闭合括号.
                    writer.Flush();
                    writer.Close();
                }
            }
            WriteLog?.Invoke(Language.GetString("dao_code_generate_completed"));
        }
    }
}
