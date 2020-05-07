using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Wunion.DataAdapter.NetCore.Test
{
    /// <summary>
    /// 用于生成数据库类型下接选框的标记.
    /// </summary>
    [HtmlTargetElement("database-collection")]
    public class DatabaseCollectionTagHelper : TagHelper
    {
        /// <summary>
        /// Http 请求上下文对象.
        /// </summary>
        public HttpContext httpContext { get; set; }

        /// <summary>
        /// 为标记生成的客户端 ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// css 样式名称.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 生成到客户端的 layui 过滤器名称.
        /// </summary>
        public string LayFilter { get; set; }

        /// <summary>
        /// 获取当前正在使用的数据库.
        /// </summary>
        /// <returns></returns>
        private string GetUsingDatabase()
        {
            DatabaseCollection dbCollection = httpContext.RequestServices.GetService(typeof(DatabaseCollection)) as DatabaseCollection;
            if (dbCollection == null)
                return string.Empty;
            return dbCollection.CurrentDbType;
        }

        /// <summary>
        /// 生成内容.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;
            if (!string.IsNullOrEmpty(Id))
                output.Attributes.Add("id", Id);
            if (!string.IsNullOrEmpty(Class))
                output.Attributes.Add("class", Class);
            if (!string.IsNullOrEmpty(LayFilter))
                output.Attributes.Add("lay-filter", LayFilter);

            context.Items.Add("ActiveDatabase", GetUsingDatabase());
            output.Content.SetHtmlContent(await output.GetChildContentAsync());
        }
    }

    /// <summary>
    /// database-collection 的选项.
    /// </summary>
    [HtmlTargetElement("item-database", ParentTag = "database-collection", TagStructure = TagStructure.WithoutEndTag)]
    public class ItemDatabaseTagHelper : TagHelper
    { 
        /// <summary>
        /// 数据库种类的名称.
        /// </summary>
        public string DbKind { get; set; }

        /// <summary>
        /// 用于显示的文本字符串.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 生成.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string useDatabase = context.Items["ActiveDatabase"] as string;
            output.TagName = "option";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("value", DbKind);
            if (!string.IsNullOrEmpty(useDatabase) && useDatabase == DbKind)
                output.Attributes.Add("selected", "selected");
            output.Content.SetContent(Text);
        }
    }
}
