using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly AuthorizationAccessor authAccessor;

        /// <summary>
        /// 创建一个 <see cref="DataProtectionController"/> 控制器实例.
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="accessor"></param>
        public DataProtectionController(IDataProtection dp, AuthorizationAccessor accessor)
        {
            protection = dp;
            authAccessor = accessor;
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
