using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
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

        /// <summary>
        /// 用于添加分组信息.
        /// </summary>
        /// <param name="name">分组名称.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([FromForm] string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(new WebApiResult<object> { code = ResultCode.STATE_FAIL, message = "未指定分组名称." });
            GroupDataService service = DataService.Get<GroupDataService>(dbCollection.Current);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("GroupName", name);
            service.Add(data, service.tableName);
            return Json(new WebApiResult<object> { code = ResultCode.STATE_OK, message = string.Empty });
        }

        /// <summary>
        /// 用于删除分组信息.
        /// </summary>
        /// <param name="Id">分组ID.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/group/{Id:int}/delete")]
        public IActionResult Delete([FromRoute] int Id)
        {
            int count = 0;
            using (DBTransactionController trans = dbCollection.Current.BeginTrans())
            {
                GroupDataService groupService = DataService.Get<GroupDataService>(dbCollection.Current);
                TestDataItemService itemService = DataService.Get<TestDataItemService>(dbCollection.Current);
                itemService.Delete(itemService.ItemsTable, new object[] { td.Field("GroupId") == Id }, trans);
                count = groupService.Delete(groupService.tableName, new object[] { td.Field("GroupId") == Id }, trans);
                if (count > 0)
                    trans.Commit();
            }
            if (count < 1)
                return Json(new WebApiResult<object> { code = ResultCode.STATE_FAIL, message = "删除信息删除失败！" });
            return Json(new WebApiResult<object> { code = ResultCode.STATE_OK, message = string.Empty });
        }
    }
}
