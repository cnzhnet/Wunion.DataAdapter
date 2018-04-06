using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Wunion.DataAdapter.NetCore.Demo.Controllers
{
    public abstract class BaseController : Controller
    {
        private IHostingEnvironment _HostingEnvironment;

        /// <summary>
        /// 创建一个 <see cref="BaseController"/> 对象的实例。
        /// </summary>
        /// <param name="hosting">用于获取Web路径映射的对象（由 ASP.NET Core传入）</param>
        protected BaseController(IHostingEnvironment hosting)
        {
            _HostingEnvironment = hosting;
        }

        /// <summary>
        /// 获取用于映射Web路径到物理路径的对象
        /// </summary>
        protected IHostingEnvironment HostingEnvironment
        {
            get { return _HostingEnvironment; }
        }

        /// <summary>
        /// 用于读取配置文件的快捷方式。
        /// </summary>
        public static IConfiguration Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// 获取通过 POST 信道提交的表单数据。
        /// </summary>
        /// <param name="Converter">针对各个表单数据进行目标数据类型转换</param>
        /// <returns></returns>
        protected Dictionary<string, object> GetPostData(DataConverterHandler Converter = null)
        {
            if (HttpContext.Request.Method.ToUpper() != "POST")
                return null;
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (HttpContext.Request.Form.Count > 0)
            {
                KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> item;
                string itemVal;
                for (int i = 0; i < HttpContext.Request.Form.Count; ++i)
                {
                    item = HttpContext.Request.Form.ElementAt(i);
                    itemVal = item.Value;
                    if (Converter == null)
                        data.Add(item.Key, itemVal);
                    else
                        data.Add(item.Key, Converter(item.Key, itemVal));
                }
            }
            return data;
        }
    }

    public delegate object DataConverterHandler(string Key, string Value);
}
