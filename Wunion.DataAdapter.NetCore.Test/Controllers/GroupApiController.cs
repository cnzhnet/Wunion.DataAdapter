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
    /// 用于提供测试分组数据相关 webapi 的控制器.
    /// </summary>
    [ServiceFilter(typeof(WebApiExceptionFilter))]
    [Route("/api/group/[action]")]
    public class GroupApiController : Controller
    {
        private DatabaseCollection dbCollection;

        /// <summary>
        /// 创建一个 <see cref="GroupApiController"/> 的对象实例.
        /// </summary>
        /// <param name="database"></param>
        public GroupApiController(DatabaseCollection database)
        {
            dbCollection = database;
        }

        /// <summary>
        /// 获取所有测试数据的分组信息.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Query()
        {
            GroupDataService service = DataService.Get<GroupDataService>(dbCollection.Current);
            List<dynamic> queryResult = service.Query();
            return Json(new WebApiResult<List<dynamic>> { 
                code = ResultCode.STATE_OK,
                data = queryResult
            });
        }
    }
}
