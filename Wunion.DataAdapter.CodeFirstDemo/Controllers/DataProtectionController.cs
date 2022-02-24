using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;
using Wunion.DataAdapter.CodeFirstDemo.Data.Security;

namespace Wunion.DataAdapter.CodeFirstDemo.Controllers
{
    /// <summary>
    /// 用于提供数据保护服务的 WebApi 控制器.
    /// </summary>
    [ApiController]
    [Route("/[controller]")]
    [ServiceFilter(typeof(WebApiExceptionFilter))]
    public class DataProtectionController : ControllerBase
    {
        private readonly IDataProtection protection;
        private readonly IDatabaseContainer dbContainer;
        private readonly AuthorizationAccessor authAccessor;

        /// <summary>
        /// 创建一个 <see cref="DataProtectionController"/> 控制器实例.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="dp"></param>
        /// <param name="accessor"></param>
        public DataProtectionController(IDatabaseContainer container, IDataProtection dp, AuthorizationAccessor accessor)
        {
            dbContainer = container;
            protection = dp;
            authAccessor = accessor;
        }

        /// <summary>
        /// 切换当前正在使用的数据库.
        /// </summary>
        /// <param name="kind">数据库种类（取值范围：mssql, mysql, npgsql, sqlite3）.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpGet, Route("/[controller]/ChangeDatabase/{kind}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult ChangeDatabase([FromRoute] string kind)
        { 
            if (string.IsNullOrEmpty(kind))
                throw new ArgumentNullException(nameof(kind));

            dbContainer.DbKind = kind;
            return new JsonResult(new ResultMessage { Code = 0x00, Message = $"已切换至 {kind} 数据库." });
        }

        /// <summary>
        /// 生成密钥.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/[controller]/GenKey")]
        [UserAuthorize(RequiredPermission = SystemPermissions.DATA_PROTECTION_CK)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult GenKey()
        {
            string key = protection.GenerateKey();
            return Content(key, "text/plain");
        }

        /// <summary>
        /// 加密保护给定的文本.
        /// </summary>
        /// <param name="text">要保护的文本.</param>
        /// <returns></returns>
        [HttpPost, Route("/[controller]/Encrypt")]
        [Consumes("application/x-www-form-urlencoded")]
        [UserAuthorize(RequiredPermission = SystemPermissions.DATA_PROTECTION_USE)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult Encrypt([FromForm] string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            return Content(protection.Encrypt(text), "text/plain");
        }

        /// <summary>
        /// 解密给定的文本.
        /// </summary>
        /// <param name="text">待解密的文本.</param>
        /// <returns></returns>
        [HttpPost, Route("/[controller]/Decrypt")]
        [Consumes("application/x-www-form-urlencoded")]
        [UserAuthorize(RequiredPermission = SystemPermissions.DATA_PROTECTION_USE)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultMessage))]
        public IActionResult Decrypt([FromForm] string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            return Content(protection.Decrypt(text), "text/plain");
        }
    }
}
