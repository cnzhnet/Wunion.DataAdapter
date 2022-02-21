using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;

namespace Wunion.DataAdapter.CodeFirstDemo.Controllers
{
    /// <summary>
    /// 提供用户账户管理功能的控制器.
    /// </summary>
    [ApiController]
    [Route("/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly IDatabaseContainer _DbContainer;

        public UserAccountController(IDatabaseContainer dbContainer, ILogger<UserAccountController> logger)
        {
            _logger = logger;
            _DbContainer = dbContainer;
        }

        /// <summary>
        /// 用于创建用户账户.
        /// </summary>
        /// <param name="account">用户账户信息.</param>
        /// <returns></returns>
        [HttpPost, Route("/[controller]/Create")]
        [Consumes("application/json", "text/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAccount))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorMessage))]
        public IActionResult Create(UserAccount account)
        {
            MyDbContext myDb = _DbContainer.GetDbContext<MyDbContext>();
            return new JsonResult(account);
        }
    }
}
