using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Linq;

namespace TeleprompterConsole
{
    /// <summary>
    /// 表示多语言支持提供程序的实现接口.
    /// </summary>
    public interface ILanguageProvider
    {
        /// <summary>
        /// 获取区域语言的名称.
        /// </summary>
        string RegionName { get; }

        /// <summary>
        /// 从语言资源中获取指定名称的字符串.
        /// </summary>
        /// <param name="name">资源的名称.</param>
        /// <returns></returns>
        string GetString(string name);

        /// <summary>
        /// 从语言资源中获取指定名称的字符串，并应用格式化参数.
        /// </summary>
        /// <param name="name">资源的名称.</param>
        /// <param name="args">参数数组.</param>
        /// <returns></returns>
        string GetString(string name, params object[] args);
    }

    /// <summary>
    /// 区域语言提供程序.
    /// </summary>
    public class LanguageProvider : ILanguageProvider
    {
        private XDocument document;
        private string Root => Program.GetBasePath();

        /// <summary>
        /// 创建一个 <see cref="LanguageProvider"/> 语言提供程序.
        /// </summary>
        /// <param name="locale">语言环境名称（例如：zh-CN、zh-HK、en-US 等）.</param>
        internal LanguageProvider(string locale)
        {
            if (string.IsNullOrEmpty(locale))
                locale = "zh-CN";
            string resourceFile = Path.Combine(Root, "lang", $"{locale}.xml");
            using (TextReader reader = new StreamReader(resourceFile, Encoding.UTF8))
            {
                document = XDocument.Load(reader);
            }
        }

        /// <summary>
        /// 获取区域语言的名称.
        /// </summary>
        public string RegionName => GetString("region-name");

        /// <summary>
        /// 从语言资源中获取指定名称的字符串.
        /// </summary>
        /// <param name="name">资源的名称.</param>
        /// <returns></returns>
        public string GetString(string name)
        {
            XElement element = document.Root.Elements("string")
                                       .Where(p => p.Attribute("name").Value == name)
                                       .FirstOrDefault();
            if (element == null)
                return string.Empty;
            return element.Attribute("value").Value;
        }

        /// <summary>
        /// 从语言资源中获取指定名称的字符串，并应用格式化参数.
        /// </summary>
        /// <param name="name">资源的名称.</param>
        /// <param name="args">参数数组.</param>
        /// <returns></returns>
        public string GetString(string name, params object[] args)
        {
            string result = GetString(name);
            if (string.IsNullOrEmpty(result))
                return result;
            return string.Format(result, args);
        }
    }
}
