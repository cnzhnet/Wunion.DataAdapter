using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.NetCore.Test.Models;

namespace Wunion.DataAdapter.NetCore.Test.Controllers
{
    /// <summary>
    /// 用于提供测试数据服务的 webapi 控制器.
    /// </summary>
    [ServiceFilter(typeof(WebApiExceptionFilter))]
    [Route("/api/data/[action]")]
    public class TestDataApiController : Controller
    {
        private DatabaseCollection db;

        /// <summary>
        /// 创建一个 <see cref="TestDataApiController"/> 的对象实例.
        /// </summary>
        /// <param name="database"></param>
        public TestDataApiController(DatabaseCollection database)
        {
            db = database;
        }

        public IActionResult List()
        {
            return Json(new WebApiResult<object> { code = ResultCode.STATE_OK, message = "webapi 请求成功！" });
        }
    }
}
