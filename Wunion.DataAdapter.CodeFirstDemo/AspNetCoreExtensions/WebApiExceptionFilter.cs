using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;
using Microsoft.AspNetCore.Http;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    /// <summary>
    /// 用于自动处理 WebApi 的异常.
    /// </summary>
    public class WebApiExceptionFilter : Attribute, IExceptionFilter
    {
        private ILogger logger;
        private IWebHostEnvironment environment;

        /// <summary>
        /// 创建一个 <see cref="WebApiExceptionFilter"/> 的对象实例.
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="env"></param>
        public WebApiExceptionFilter(ILogger<WebApiExceptionFilter> _logger, IWebHostEnvironment env)
        {
            logger = _logger;
            environment = env;
        }

        /// <summary>
        /// WebApi 中产生错误时执行.
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            WebApiException webApiEx = context.Exception as WebApiException;
            ResultMessage message = new ResultMessage();
            if (webApiEx != null)
            {
                message.Code = webApiEx.Code;
                message.Message = webApiEx.Message;
            }
            else
            {
                message.Code = 500;
                message.Message = context.Exception.Message;
            }
            JsonSerializerOptions options = new JsonSerializerOptions {
                PropertyNamingPolicy = null,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            context.Result = new JsonResult(message, options) { 
                StatusCode = StatusCodes.Status500InternalServerError 
            };
            context.ExceptionHandled = true;
        }
    }
}
