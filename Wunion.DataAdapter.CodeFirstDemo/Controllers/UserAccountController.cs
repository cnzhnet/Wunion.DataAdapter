using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;
using Wunion.DataAdapter.CodeFirstDemo.Data.Security;
using Wunion.DataAdapter.CodeFirstDemo.Services;
using Microsoft.Extensions.Options;

namespace Wunion.DataAdapter.CodeFirstDemo.Controllers
{
    /// <summary>
    /// 提供用户账户管理功能的控制器.
    /// </summary>
    [ApiController]
    [Route("/[controller]")]
    [ServiceFilter(typeof(WebApiExceptionFilter))]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly IDatabaseContainer _DbContainer;
        private readonly UserAccountService service;
        private readonly IDataProtection dp;
        private readonly IConfiguration configuration;
        private readonly JsonOptions jsonOptions;
        private readonly AuthorizationAccessor authAccessor;

        public UserAccountController(IDatabaseContainer dbContainer, 
            IDataProtection _dp, 
            AuthorizationAccessor accessor, 
            IConfiguration conf, 
            IOptions<JsonOptions> jsonOpt, 
            ILogger<UserAccountController> logger)
        {
            _logger = logger;
            _DbContainer = dbContainer;
            dp = _dp;
            authAccessor = accessor;
            configuration = conf;
            jsonOptions = jsonOpt.Value;
            service = new UserAccountService(dbContainer.GetDbContext<MyDbContext>());
        }

        /// <summary>
        /// 获取所有用户账户列表.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/[controller]/Get")]
        [UserAuthorize(RequiredPermission = SystemPermissions.USER_ACCOUNT_RD)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDataModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult Get()
        {
            return new JsonResult(service.List());
        }

        /// <summary>
        /// 获取指定 UID 的用户账户的详细信息.
        /// </summary>
        /// <param name="uid">用户账户的 UID.</param>
        /// <returns></returns>
        [HttpGet, Route("/[controller]/{uid:int}/Get")]
        [UserAuthorize(RequiredPermission = SystemPermissions.USER_ACCOUNT_RD)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDataModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult GetById([FromRoute] int uid)
        {
            UserDataModel user = service.GetById(uid);
            if (user == null)
            {
                return new JsonResult(new ResultMessage { Code = 404, Message = "未找到指定的用户" }) { 
                    StatusCode = StatusCodes.Status404NotFound 
                };
            }
            user.Password = string.Empty;
            return new JsonResult(user);
        }

        /// <summary>
        /// 用于创建用户账户.
        /// </summary>
        /// <param name="user">用户账户信息.</param>
        /// <returns></returns>
        [HttpPost, Route("/[controller]/Create")]
        [Consumes("application/json", "text/json")]
        [UserAuthorize(RequiredPermission = SystemPermissions.USER_ACCOUNT_CT)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDataModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult Create(UserDataModel user)
        {
            if (user == null)
                throw new WebApiException(1002, "No post data or the data is wrong.");

            service.Add(user);
            return new JsonResult(user);
        }

        /// <summary>
        /// 删除指定 uid 的用户账户.
        /// </summary>
        /// <param name="uid">用户账户的 uid</param>
        /// <returns></returns>
        [HttpDelete, Route("/[controller]/{uid:int}/Delete")]
        [UserAuthorize(RequiredPermission = SystemPermissions.USER_ACCOUNT_RM)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult Delete([FromRoute] int uid)
        {
            if (uid == 0)
                throw new WebApiException(2003, "无法删除超级用户.");
            if (uid == authAccessor.Authorization.UID)
                throw new WebApiException(2003, "不能删除正在使用的用户.");
            service.Delete(uid);
            return new JsonResult(new ResultMessage { 
                Code = 0x00, Message = "删除完成，但不知是否成功."
            });
        }

        /// <summary>
        /// 用户账户登录.
        /// </summary>
        /// <param name="name">用户账户名或邮箱.</param>
        /// <param name="password">登录密码.</param>
        /// <returns></returns>
        [HttpPost, Route("/[controller]/LogIn")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorizationMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult LogIn([FromForm] string name, [FromForm] string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                throw new WebApiException(500, "用户名称或密码未指定.");

            UserAuthorization authInfo = service.LogIn(name, password);
            IConfigurationSection section = configuration.GetSection("Authorization");
            authInfo.Producer = section.GetValue<string>("producer");
            authInfo.Period = section.GetValue<int>("period");
            authInfo.Grant = DateTime.Now;
            string token = JsonSerializer.Serialize<UserAuthorization>(authInfo, jsonOptions.JsonSerializerOptions);
            token = dp.Encrypt(token);
            return new JsonResult(new AuthorizationMessage { 
                Grant = authInfo.Grant,
                Period = authInfo.Period,
                Token = token
            });
        }
    }
}
