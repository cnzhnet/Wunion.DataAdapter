using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Wunion.DataAdapter.NetCore.Test.Models
{
    /// <summary>
    /// 表示模块视图模型.
    /// </summary>
    public class ModuleViewModel
    {
        /// <summary>
        /// 创建一个 <see cref="ModuleViewModel"/> 的对象实例.
        /// </summary>
        public ModuleViewModel() { }

        /// <summary>
        /// 获取或设置视图名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置 http 请求上下文.
        /// </summary>
        public HttpContext Context { get; set; }

        /// <summary>
        /// 获取指定名称的查询参数.
        /// </summary>
        /// <param name="name">参数名称.</param>
        /// <returns></returns>
        public string QueryString(string name)
        {
            if (!(Context.Request.Query.ContainsKey(name)))
                return string.Empty;
            return Context.Request.Query[name];
        }
    }
}
