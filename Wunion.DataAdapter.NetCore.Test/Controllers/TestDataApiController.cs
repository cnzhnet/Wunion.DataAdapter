using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.NetCore.Test.Models;
using Wunion.DataAdapter.NetCore.Test.Services;

namespace Wunion.DataAdapter.NetCore.Test.Controllers
{
    /// <summary>
    /// 用于提供测试数据服务的 webapi 控制器.
    /// </summary>
    [ServiceFilter(typeof(WebApiExceptionFilter))]
    [Route("/api/data/[action]")]
    public class TestDataApiController : Controller
    {
        private DatabaseCollection dbCollection;

        /// <summary>
        /// 创建一个 <see cref="TestDataApiController"/> 的对象实例.
        /// </summary>
        /// <param name="database"></param>
        public TestDataApiController(DatabaseCollection database)
        {
            dbCollection = database;
        }

        /// <summary>
        /// 以分页的形式获取数据列表.
        /// </summary>
        /// <param name="page">当前页.</param>
        /// <param name="pagesize">每页的数据条数.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult List([FromForm] int page, [FromForm] int pagesize)
        {
            int? groupId = null;
            if (HttpContext.Request.Form.ContainsKey("group"))
            {
                int tmp = -1;
                if (int.TryParse(HttpContext.Request.Form["group"].ToString(), out tmp))
                    groupId = tmp;
            }
            TestDataItemService service = DataService.Get<TestDataItemService>(dbCollection.Current);
            PaginatedDataCollection<dynamic> queryResult = service.Query(page, pagesize, groupId);
            if (queryResult == null)
                return Json(new WebApiResult<object> { code = ResultCode.STATE_FAIL, message = "还没有测试数据." });
            return Json(new WebApiResult<PaginatedDataCollection<dynamic>> { 
                code = ResultCode.STATE_OK,
                data = queryResult
            });
        }
    }
}
