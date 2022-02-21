using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TeleprompterConsole.ProjectAnalysis
{
    /// <summary>
    /// 表示目标项目信息.
    /// </summary>
    public class ProjectInfo
    { 
        /// <summary>
        /// 项目生成的程序集名称.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 目标框架.
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        /// 目标运行环境.
        /// </summary>
        public string RuntimeIdentifier { get; set; }

        /// <summary>
        /// 程序集的生成路径.
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// 代码注释文件.
        /// </summary>
        public string CommentsFile { get; set; }

        public override string ToString()
        {
            return $"AssemblyName: {AssemblyName}\r\nTargetFramework: {TargetFramework}\r\nRuntimeIdentifier: {RuntimeIdentifier}\r\n";
        }
    }

    /// <summary>
    /// 表示目标项目分析器.
    /// </summary>
    internal class ProjectAnalyzer
    {
        private XDocument csproj;
        private ILanguageProvider language;
        private List<string> supportedFrameworks;

        internal ProjectAnalyzer(string root, ILanguageProvider lang)
        {
            RootDir = root;
            language = lang;
            supportedFrameworks = new List<string>(new string[] {
                "netcoreapp2.0", "netcoreapp2.1", "netcoreapp2.2", "netcoreapp3.0", "netcoreapp3.1", "net5.0", "net6.0"
            });
            Project = null;
        }

        /// <summary>
        /// 目标项目的根目录.
        /// </summary>
        internal string RootDir { get; private set; }

        /// <summary>
        /// 分析的项目信息.
        /// </summary>
        internal ProjectInfo Project { get; private set; }

        /// <summary>
        /// 运行目标项目分析.
        /// </summary>
        internal void Run()
        {
            Project = null;
            // 读取项目文件 *.csproj
            DirectoryInfo root = new DirectoryInfo(RootDir);
            FileInfo projFile = root.GetFiles("*.csproj").FirstOrDefault();
            if (projFile == null)
                throw new FileNotFoundException(language.GetString("no_project"));
            using (FileStream stream = projFile.OpenRead())
                csproj = XDocument.Load(stream);

            // 从项目文件中读取必要的项目信息.
            ProjectInfo info = new ProjectInfo();
            IEnumerable<XElement> propertyGroups = csproj.Root.Elements("PropertyGroup");
            XElement elem;
            string targetFrameworks = null;
            foreach (XElement group in propertyGroups)
            {
                elem = group.Element("TargetFramework");
                if (elem != null)
                    info.TargetFramework = elem.Value;
                elem = group.Element("TargetFrameworks");
                if (elem != null)
                    targetFrameworks = elem.Value;
                elem = group.Element("RuntimeIdentifier");
                if (elem != null)
                    info.RuntimeIdentifier = elem.Value;
                elem = group.Element("AssemblyName");
                if (elem != null)
                    info.AssemblyName = elem.Value;
            }
            if (string.IsNullOrEmpty(info.TargetFramework) && !string.IsNullOrEmpty(targetFrameworks))
                info.TargetFramework = targetFrameworks.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (string.IsNullOrEmpty(info.TargetFramework))
                throw new Exception(language.GetString("target_framework_missing"));
            if (!supportedFrameworks.Contains(info.TargetFramework))
                throw new NotSupportedException(language.GetString("target_framework_not_supported"));

            if (string.IsNullOrEmpty(info.AssemblyName))
                info.AssemblyName = Path.GetFileNameWithoutExtension(projFile.Name);
            info.OutputPath = Path.Combine(RootDir, "bin", "Debug", info.TargetFramework);
            if (!string.IsNullOrEmpty(info.RuntimeIdentifier))
                info.OutputPath = Path.Combine(info.OutputPath, info.RuntimeIdentifier);
            if (!string.IsNullOrEmpty(info.OutputPath))
                info.CommentsFile = Path.Combine(info.OutputPath, $"{info.AssemblyName}.xml");
            else
                info.CommentsFile = Path.Combine(RootDir, $"{info.AssemblyName}.xml");

            Project = info;
        }
    }
}
