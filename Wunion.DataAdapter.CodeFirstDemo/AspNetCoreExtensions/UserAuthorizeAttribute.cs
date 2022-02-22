using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;
using Wunion.DataAdapter.CodeFirstDemo.Data.Security;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    /// <summary>
    /// 用于验证用户账的授权，若不通过则阻断访问.
    /// </summary>
    public class UserAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 超级用户的 UID.
        /// </summary>
        private const int SUPER_USER_UID = 0;
        private const string AUTH_NAME = "authorization";

        public UserAuthorizeAttribute()
        {
            RequiredPermission = -1;
        }

        /// <summary>
        /// 获取或设置验证时所需要的权限.
        /// </summary>
        public int RequiredPermission { get; set; }

        /// <summary>
        /// 授权通过返回 true，否则应返回 false .
        /// </summary>
        /// <param name="authorization">授权信息.</param>
        /// <returns></returns>
        protected virtual bool OnAuthorize(UserAuthorization authorization)
        {
            DateTime lastTime = authorization.Grant.AddMinutes(authorization.Period);
            if (lastTime < DateTime.Now)
                return false;
            if (authorization.UID == SUPER_USER_UID) // 超级管理员放行所有权限.
                return true;
            return authorization.Permissions.Contains(RequiredPermission);
        }

        /// <summary>
        /// 没有权限时阻断请求.
        /// </summary>
        /// <param name="context"></param>
        protected void PermissionDenied(ActionExecutingContext context)
        {
            JsonResult result = new JsonResult(new ResultMessage{
                Code = StatusCodes.Status403Forbidden,
                Message = "没有权限."
            });
            result.StatusCode = StatusCodes.Status403Forbidden;
            context.Result = result;
        }

        /// <summary>
        /// 执行访问授权的验证.
        /// </summary>
        /// <param name="context"></param>
        public sealed override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                // 从 http 头中获取授权信息.
                StringValues buffer = string.Empty;
                if (!context.HttpContext.Request.Headers.TryGetValue(AUTH_NAME, out buffer))
                {
                    PermissionDenied(context);
                    return;
                }
                string token = buffer.ToString();
                if (string.IsNullOrEmpty(token))
                {
                    PermissionDenied(context);
                    return;
                }
                // 解释授权信息.
                string[] array = token.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                token = array.Last();
                IDataProtection dp = context.HttpContext.RequestServices.GetService(typeof(IDataProtection)) as IDataProtection;
                IOptions<JsonOptions> options = context.HttpContext.RequestServices.GetService(typeof(IOptions<JsonOptions>)) as IOptions<JsonOptions>;
                AuthorizationAccessor authAccessor = context.HttpContext.RequestServices.GetService(typeof(AuthorizationAccessor)) as AuthorizationAccessor;
                token = dp.Decrypt(token);
                UserAuthorization authorization = JsonSerializer.Deserialize<UserAuthorization>(token, options.Value.JsonSerializerOptions);
                if (!OnAuthorize(authorization))
                {
                    PermissionDenied(context);
                    return;
                }
                authAccessor.Authorization = authorization;
            }
            catch (Exception Ex)
            {
                context.Result = new JsonResult(new ResultMessage { Code = 10034, Message = $"给定的授权信息无效：{Ex.Message}" }) { 
                    StatusCode = StatusCodes.Status500InternalServerError 
                };
            }
        }
    }
}
