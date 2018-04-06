using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Services
{
    /// <summary>
    /// 表示语言支持服务.
    /// </summary>
    public class LanguageService : ICodeCommentsLanguage
    {
        /// <summary>
        /// 语言支持的 xml 文档.
        /// </summary>
        private XDocument document;
        private string ResultFile;
        private string _RegionName;
        private List<LocaleResourceItem> UIResources;

        /// <summary>
        /// 创建一个 <see cref="LanguageService"/> 的对象实例.
        /// </summary>
        /// <param name="resourceContent">语言的资源内容.</param>
        protected LanguageService(string resourceContent)
        {
            document = XDocument.Load(new StringReader(resourceContent));
            ResultFile = resourceContent;
        }

        /// <summary>
        /// 获取语言环境名称.
        /// </summary>
        public string RegionName
        {
            get
            {
                if (string.IsNullOrEmpty(_RegionName))
                    _RegionName = GetString("region-name");
                return _RegionName;
            }
        }

        /// <summary>
        /// 获取指定键的字符串.
        /// </summary>
        /// <param name="key">资源键的名称.</param>
        /// <returns></returns>
        public string GetString(string key)
        {
            IEnumerable<XElement> Result = from elem in document.Root.Elements("string")
                                           where elem.Attribute("key").Value == key
                                           select elem;
            if (Result == null)
                return null;
            if (Result.Count() < 1)
                return null;
            return Result.First().Attribute("value").Value;
        }

        /// <summary>
        /// 获取语言的所有项目.
        /// </summary>
        /// <returns></returns>
        public List<LocaleResourceItem> GetUiResources()
        {
            if (UIResources != null)
                return UIResources;
            UIResources = new List<LocaleResourceItem>();
            LocaleResourceItem item;
            IEnumerable<XElement> Result = from p in document.Root.Elements("string")
                                           where p.Attribute("bind-ui").Value.ToLower() == "true"
                                           select p;
            foreach (XElement elem in Result)
            {
                if (elem.Attribute("key").Value == "region-name")
                    continue;
                item = new LocaleResourceItem();
                item.Key = elem.Attribute("key").Value;
                item.Value = elem.Attribute("value").Value;
                item.forUI = true;
                if (elem.Attribute("format") != null)
                    item.Format = elem.Attribute("format").Value.ToLower() == "true";
                UIResources.Add(item);
            }
            return UIResources;
        }

        /// <summary>
        /// 将语言服务的环境名称输出.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RegionName;
        }

        /// <summary>
        /// 加载语言指定的语言资源.
        /// </summary>
        /// <param name="localeName">区域语言名称.</param>
        /// <returns></returns>
        public static LanguageService LoadResource(string localeName)
        {
            if (string.IsNullOrEmpty(localeName))
                return new LanguageService(global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.zh_CN);
            switch (localeName.ToLower())
            {
                case "en-us":
                    return new LanguageService(global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.en_US);
                default:
                    return new LanguageService(global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.zh_CN);
            }
        }

        /// <summary>
        /// 获取所有当前可用的语言支持.
        /// </summary>
        /// <returns></returns>
        public static List<LanguageService> GetAvailables()
        {
            List<LanguageService> services = new List<LanguageService>();
            services.Add(new LanguageService(global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.zh_CN));
            services.Add(new LanguageService(global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.en_US));
            return services;
        }
    }

    /// <summary>
    /// 表示语言环境资源项的对象类型.
    /// </summary>
    public class LocaleResourceItem
    {
        /// <summary>
        /// 创建一个 <see cref="LocaleResourceItem"/> 的对象实例.
        /// </summary>
        public LocaleResourceItem() { }

        /// <summary>
        /// 获取或设置资源项是否为 UI 界面元素所有.
        /// </summary>
        public bool forUI { get; set; }

        /// <summary>
        /// 获取或设置资源的键名称.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 获取或设置资源的字符串值.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 获取或设置资源的 Value 值是否需要格式化输出.
        /// </summary>
        public bool Format { get; set; }
    }
}
