using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    /// <summary>
    /// 表示 webapi 中触发的异常.
    /// </summary>
    public class WebApiException : Exception
    {
        /// <summary>
        /// 创建一个 <see cref="WebApiException"/> 的对象实例.
        /// </summary>
        /// <param name="code">错误代码.</param>
        /// <param name="message">错误信息.</param>
        /// <param name="innerException">引发当前错误的异常信息.</param>
        public WebApiException(int code, string message, Exception innerException = null) 
            : base(message, innerException)
        {
            Code = code;
        }

        /// <summary>
        /// 已知错误的异常代码.
        /// </summary>
        public int Code { get; private set; }
    }
}
