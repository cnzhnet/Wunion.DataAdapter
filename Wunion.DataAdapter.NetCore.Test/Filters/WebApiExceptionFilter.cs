using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Wunion.DataAdapter.NetCore.Test.Models;

namespace Wunion.DataAdapter.NetCore.Test
{
    /// <summary>
    /// 用于处理开发人员未处理的错误的过滤器。
    /// </summary>
    public class WebApiExceptionFilter : Attribute, IExceptionFilter
    {
        private ILogger Logger;
        private IWebHostEnvironment environment;

        /// <summary>
        /// 创建一个 <see cref="WebApiExceptionFilter"/> 的对象实例.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        public WebApiExceptionFilter(ILogger<WebApiExceptionFilter> logger, IWebHostEnvironment env)
        {
            Logger = logger;
            environment = env;
        }

        /// <summary>
        /// 当产生未处理的异常时执行.
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            Logger?.LogError(context.Exception, context.Exception.Message);
            WebApiResult<object> msg = new WebApiResult<object> {
                code = ResultCode.SERVER_ERROR,
                message = context.Exception.Message
            };
            JsonSerializerOptions options = new JsonSerializerOptions {
                PropertyNamingPolicy = null,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            context.Result = new ContentResult {
                Content = JsonSerializer.Serialize<WebApiResult<object>>(msg, options),
                ContentType = "application/json;charset=utf-8"
            };
            context.ExceptionHandled = true;
        }
    }
}
