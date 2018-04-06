using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CSharp;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandParser;
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Services
{
    /// <summary>
    /// 生成器服务.
    /// </summary>
    public class GeneratorService
    {
        private DataEngine _DbEngine;
        private List<TableInfoModel> _AllTables;

        /// <summary>
        /// 创建一个生成器实例.
        /// </summary>
        /// <param name="DBA"></param>
        /// <param name="adapter"></param>
        /// <param name="context"></param>
        public GeneratorService(DbAccess DBA, ParserAdapter adapter)
        {
            _DbEngine = new DataEngine(DBA, adapter);
        }

        /// <summary>
        /// 获取数据引擎.
        /// </summary>
        public DataEngine DbEngine => _DbEngine;

        /// <summary>
        /// 获取或设置数据库所有表信息.
        /// </summary>
        public List<TableInfoModel> AllTables
        {
            get { return GetAllTables(); }
        }

        /// <summary>
        /// 获取或设置当前的语言支持服务.
        /// </summary>
        public LanguageService Language { get; set; }

        /// <summary>
        /// 获取或设置数据库上下文对象.
        /// </summary>
        public IDatabaseContext DbContext { get; set; }

        /// <summary>
        /// 获取或设置生成代码的输出目录.
        /// </summary>
        public string OutputDir { get; set; }

        /// <summary>
        /// 获取或设置代码生成服务的生成模式.
        /// </summary>
        public BuildPattern Pattern { get; set; }

        /// <summary>
        /// 获取所有表数据.
        /// </summary>
        /// <returns></returns>
        private List<TableInfoModel> GetAllTables()
        {
            if (_AllTables != null && _AllTables.Count > 1)
                return _AllTables;
            _AllTables = DbContext.GetTables();
            return _AllTables;
        }

        /// <summary>
        /// 当生成进度发生改变时触发此事件.
        /// </summary>
        public event BuildProgressEventHandler ProgressChange;

        /// <summary>
        /// 用于执行生成进度事件的委托.
        /// </summary>
        /// <param name="percentage">进度百分比值.</param>
        /// <param name="message">与进度相关的文本信息.</param>
        protected virtual void OnProgressChange(int percentage, string message)
        {
            if (ProgressChange != null)
                ProgressChange(percentage, message);
        }

        /// <summary>
        /// 生成代码.
        /// </summary>
        /// <param name="codeNamespace">生成代码的命名空间.</param>
        /// <param name="tables">仅为这些表生成代码.</param>
        public void BuildTo(string codeNamespace, List<TableInfoModel> tables = null)
        {
            List<TableInfoModel> buildTables;
            if (tables != null && tables.Count > 0)
                buildTables = tables;
            else
                buildTables = AllTables.Distinct(p => p.tableName).ToList();
            if (buildTables == null || buildTables.Count < 1)
                return;
            // 创建代码生成器.
            CodeGenerator entityGenerator = null, agentGenerator = null, contGenerator = null;
            switch (Pattern)
            {
                case BuildPattern.BuildEntity:
                    entityGenerator = new EntityCodeGenerator();
                    entityGenerator.LangComments = Language;
                    break;
                case BuildPattern.BuildAgent:
                    agentGenerator = new EntityAgentCodeGenerator();
                    agentGenerator.LangComments = Language;
                    contGenerator = new EntityContextGenerator();
                    contGenerator.LangComments = Language;
                    break;
                default:
                    entityGenerator = new EntityCodeGenerator();
                    entityGenerator.LangComments = Language;
                    agentGenerator = new EntityAgentCodeGenerator();
                    agentGenerator.LangComments = Language;
                    contGenerator = new EntityContextGenerator();
                    contGenerator.LangComments = Language;
                    break;
            }
            // 开始生成代码.
            CodeClassDeclaration entityClass, contextClass, agentClass;
            List<TableInfoModel> currentTable;
            for (int i = 0; i < buildTables.Count; ++i)
            {
                int bfb = (int)(Convert.ToSingle(i) / buildTables.Count * 100);
                currentTable = AllTables.Where(item => item.tableName == buildTables[i].tableName).ToList();
                if (entityGenerator != null) // 生成实体类的代码.
                {
                    OnProgressChange(bfb, string.Format(Language.GetString("BuildingEntityClass"), buildTables[i].tableName));
                    entityClass = new CodeClassDeclaration(currentTable);
                    entityClass.BaseTypes.Add("DataEntity");
                    entityClass.CodeComment = buildTables[i].tableDescription;
                    entityClass.Namespace = codeNamespace;
                    entityClass.Name = buildTables[i].tableName;
                    entityClass.TableName = buildTables[i].tableName;
                    entityGenerator.AddClass(entityClass);
                    entityGenerator.WriteTo(Path.Combine(OutputDir, string.Format("{0}.cs", buildTables[i].tableName)), true);
                }
                if (agentGenerator != null)
                {
                    OnProgressChange(bfb, string.Format(Language.GetString("BuildingAgentClass"), buildTables[i].tableName));
                    agentClass = new CodeClassDeclaration(currentTable);
                    agentClass.BaseTypes.Add("EntityAgent");
                    agentClass.CodeComment = string.Format(Language.GetString("AgentClassComment"), buildTables[i].tableDescription);
                    agentClass.Namespace = codeNamespace;
                    agentClass.Name = string.Format("{0}Agent", buildTables[i].tableName);
                    agentClass.TableName = buildTables[i].tableName;
                    agentGenerator.AddClass(agentClass);

                    OnProgressChange(bfb, string.Format(Language.GetString("BuildingContextClass"), buildTables[i].tableName));
                    contextClass = new CodeClassDeclaration(currentTable);
                    contextClass.BaseTypes.Add(string.Format("TableContext<{0}Agent>", buildTables[i].tableName));
                    contextClass.CodeComment = string.Format(Language.GetString("ContextClassComment"), buildTables[i].tableName);
                    contextClass.Namespace = codeNamespace;
                    contextClass.Name = string.Format("{0}Context", buildTables[i].tableName);
                    contextClass.TableName = buildTables[i].tableName;
                    agentGenerator.AddClass(contGenerator.BuildClass(contextClass));
                    agentGenerator.WriteTo(Path.Combine(OutputDir, string.Format("{0}Context.cs", buildTables[i].tableName)), true);
                }
            }
        }

        /// <summary>
        /// 保存注释信息.
        /// </summary>
        /// <param name="filePath">注释文件的保存路径.</param>
        public void SaveComments(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            List<TableInfoModel> tables = AllTables.Distinct(p => p.tableName).ToList();
            XmlDocument document = new XmlDocument();
            document.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><CodeComments></CodeComments>");
            XmlNode Root = document.SelectSingleNode("/CodeComments");
            XmlElement entityClass, memberElem;
            foreach (TableInfoModel tableInfo in tables)
            {
                if (string.IsNullOrEmpty(tableInfo.tableDescription))
                    tableInfo.tableDescription = string.Empty;
                entityClass = document.CreateElement("EntityClass");
                entityClass.SetAttribute("Name", tableInfo.tableName);
                entityClass.SetAttribute("Comment", tableInfo.tableDescription);
                // 创建并存储类成员的注释
                List<TableInfoModel> classMembers = AllTables.Where(item => item.tableName == tableInfo.tableName).ToList();
                foreach (TableInfoModel m in classMembers)
                {
                    memberElem = document.CreateElement("Member");
                    memberElem.SetAttribute("Name", m.paramName);
                    memberElem.SetAttribute("Comment", m.paramDescription);
                    entityClass.AppendChild(memberElem);
                }
                Root.AppendChild(entityClass);
            }
            // 保存 xml 文档.
            if (File.Exists(filePath))
                File.Delete(filePath);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                NewLineChars = Environment.NewLine
            };
            using (Stream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                XmlWriter writer = XmlTextWriter.Create(fs, settings);
                document.WriteTo(writer);
                writer.Flush();
                writer.Close();
                fs.Flush();
            }
        }

        /// <summary>
        /// 加载注释信息.
        /// </summary>
        /// <param name="filePath">注释信息文件.</param>
        public void LoadComments(string filePath)
        {
            if (!(File.Exists(filePath)))
                return;
            XDocument xDoc = XDocument.Load(filePath);
            List<XElement> entitys = xDoc.Root.Elements("EntityClass").ToList();
            List<XElement> members;
            TableInfoModel tableInfo;
            foreach (XElement elem in entitys)
            {
                members = elem.Elements("Member").ToList();
                foreach (XElement m in members)
                {
                    tableInfo = AllTables.Where(item => item.tableName == elem.Attribute("Name").Value && item.paramName == m.Attribute("Name").Value).First();
                    if (tableInfo == null)
                        continue;
                    tableInfo.tableDescription = elem.Attribute("Comment").Value;
                    tableInfo.paramDescription = m.Attribute("Comment").Value;
                }
            }
        }

        /// <summary>
        /// 表示代码生成模式的枚举类型.
        /// </summary>
        public enum BuildPattern
        {
            /// <summary>
            /// 生成实体类及代理类.
            /// </summary>
            BuildAll = 0x00,

            /// <summary>
            /// 仅生成实体类.
            /// </summary>
            BuildEntity = 0x01,

            /// <summary>
            /// 仅生成代理类.
            /// </summary>
            BuildAgent = 0x02
        }
    }

    /// <summary>
    /// 用于执行生成进度事件的委托.
    /// </summary>
    /// <param name="percentage">进度百分比值.</param>
    /// <param name="message">与进度相关的文本信息.</param>
    public delegate void BuildProgressEventHandler(int percentage, string message);
}
