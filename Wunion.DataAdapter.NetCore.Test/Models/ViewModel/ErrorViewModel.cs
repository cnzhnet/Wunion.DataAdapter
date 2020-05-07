using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Wunion.DataAdapter.NetCore.Test.Models
{
    /// <summary>
    /// 错误页观视图模型.
    /// </summary>
    public class ErrorViewModel : ModuleViewModel
    {
        /// <summary>
        /// 创建一个 <see cref="ErrorViewModel"/> 的对象实例.
        /// </summary>
        public ErrorViewModel() { }

        /// <summary>
        /// 获取或设置错误信息的标题.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取或设置错误信息的消息内容.
        /// </summary>
        public string Message { get; set; }
    }
}
