using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.NetCore.Test.Models;
using Wunion.DataAdapter.NetCore.Test.Services;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

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
        private List<string> allowedImageExtension;

        /// <summary>
        /// 创建一个 <see cref="TestDataApiController"/> 的对象实例.
        /// </summary>
        /// <param name="database"></param>
        public TestDataApiController(DatabaseCollection database)
        {
            dbCollection = database;
            allowedImageExtension = new List<string>(new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" });
        }

        /// <summary>
        /// 以分页的形式获取数据列表的 webapi.
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
            PaginatedCollection<dynamic> queryResult = service.Query(page, pagesize, groupId);
            if (queryResult == null)
                return Json(new WebApiResult<object> { code = ResultCode.STATE_FAIL, message = "还没有测试数据." });
            return Json(new WebApiResult<PaginatedCollection<dynamic>> { 
                code = ResultCode.STATE_OK,
                data = queryResult
            });
        }

        /// <summary>
        /// 用于搜索数据的 webapi.
        /// </summary>
        /// <param name="keywords">搜索关键字.</param>
        /// <param name="page">当前页.</param>
        /// <param name="pagesize">每页的数据条数.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Search([FromForm] string keywords, [FromForm] int page, [FromForm] int pagesize)
        {
            if (string.IsNullOrEmpty(keywords))
                return Json(new WebApiResult<object> { code = ResultCode.STATE_FAIL, message = "未指定搜索关键字." });
            int? groupId = null;
            if (HttpContext.Request.Form.ContainsKey("group"))
            {
                int tmp = -1;
                if (int.TryParse(HttpContext.Request.Form["group"].ToString(), out tmp))
                    groupId = tmp;
            }
            TestDataItemService service = DataService.Get<TestDataItemService>(dbCollection.Current);
            PaginatedCollection<dynamic> queryResult = service.Search(keywords, page, pagesize, groupId);
            if (queryResult == null)
                return Json(new WebApiResult<object> { code = ResultCode.STATE_FAIL, message = string.Format("未搜索到“{0}”相关的结果.", keywords) });
            return Json(new WebApiResult<PaginatedCollection<dynamic>> { code = ResultCode.STATE_OK, data = queryResult });
        }

        /// <summary>
        /// 获取上传的图像数据.
        /// </summary>
        /// <returns></returns>
        private byte[] GetUploadPicture()
        {
            byte[] buffers = null;
            if (HttpContext.Request.Form.Files.Count < 1)
                return buffers;
            IFormFile file = null;
            foreach (IFormFile item in HttpContext.Request.Form.Files)
            {
                if (string.IsNullOrEmpty(item.Name))
                    continue;
                if (item.Name.ToLower() == "testphoto")
                {
                    file = item;
                    break;
                }
            }
            if (file == null)
                return buffers;
            string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (!allowedImageExtension.Contains(extension))
                throw new NotSupportedException(string.Format("不支持的图像类型： {0}", extension));
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                file.CopyTo(stream);
                stream.Flush();
                buffers = stream.ToArray();
                stream.Close();
            }
            return buffers;
        }

        /// <summary>
        /// 获取客户端提交的数据字典.
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> GetPostDataAsync()
        {
            Dictionary<string, object> data = null;
            if (HttpContext.Request.ContentType.ToLower().Contains("multipart/form-data"))
            {
                data = new Dictionary<string, object>();
                foreach (KeyValuePair<string, StringValues> item in HttpContext.Request.Form)
                {
                    switch (item.Key.ToLower())
                    {
                        case "TestId":
                        case "TestPhoto":
                            break;
                        case "GroupId":
                            data.Add("GroupId", Convert.ToInt32(item.Value));
                            break;
                        case "TestAge":
                            data.Add("TestAge", Convert.ToSingle(item.Value));
                            break;
                        default:
                            data.Add(item.Key, item.Value.ToString());
                            break;
                    }
                }
                byte[] picture = GetUploadPicture();
                if (picture != null)
                    data.Add("TestPhoto", picture);
            }
            else
            {
                data = await this.ReadJsonBodyToDictionary((name, elem) => {
                    switch (name)
                    {
                        case "TestId": return null;
                        case "GroupId": return elem.GetInt32();
                        case "TestAge": return elem.GetSingle();
                        case "TestPhoto": return null;
                        default: return elem.GetString();
                    }
                });
            }
            return data;
        }

        /// <summary>
        /// 用于添加测试数据记录的 webapi.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json", "text/json", "multipart/form-data")]
        public async Task<IActionResult> AddAsync()
        {
            Dictionary<string, object> data = await GetPostDataAsync();
            if (data.Count < 1)
                return Json(new WebApiResult<object> { code = ResultCode.MISSING_ARGUMENT, message = "未提交记录的数据." });
            TestDataItemService service = DataService.Get<TestDataItemService>(dbCollection.Current);
            service.Add(data, service.ItemsTable);
            return Json(new WebApiResult<object> { code = ResultCode.STATE_OK, message = string.Empty });
        }

        /// <summary>
        /// 用于更新数据的 webapi .
        /// </summary>
        /// <param name="dataId">要更新的记录ID.</param>
        /// <returns></returns>
        [Route("/api/data/{dataId:int}/update"), HttpPost]
        [Consumes("application/json", "text/json", "multipart/form-data")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int dataId)
        {
            Dictionary<string, object> data = await GetPostDataAsync();
            if (data.Count < 1)
                return Json(new WebApiResult<object> { code = ResultCode.MISSING_ARGUMENT, message = "未提交记录的数据." });
            TestDataItemService service = DataService.Get<TestDataItemService>(dbCollection.Current);
            service.Update(data, service.ItemsTable, new object[] { td.Field("TestId") == dataId });
            return Json(new WebApiResult<object> { code = ResultCode.STATE_OK, message = string.Empty });
        }

        /// <summary>
        /// 用于删除数据的 webapi.
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        [Route("/api/data/{dataId:int}/delete"), HttpGet]
        public IActionResult Delete([FromRoute] int dataId)
        {
            TestDataItemService service = DataService.Get<TestDataItemService>(dbCollection.Current);
            int count = service.Delete(service.ItemsTable, new object[] { td.Field("TestId") == dataId });
            if (count > 0)
                return Json(new WebApiResult<object> { code = ResultCode.STATE_OK, message = string.Empty });
            return Json(new WebApiResult<object> { code = ResultCode.STATE_FAIL, message = "数据删除失败." });
        }

        /// <summary>
        /// 用于获取指定图记录的图片的 webapi.
        /// </summary>
        /// <param name="dataId">记录ID.</param>
        /// <returns></returns>
        [Route("/api/data/{dataId:int}/picture"), HttpGet]
        public IActionResult GetPicture([FromRoute] int dataId)
        {
            try
            {
                TestDataItemService service = DataService.Get<TestDataItemService>(dbCollection.Current);
                byte[] buffers = service.GetPictureData(dataId);
                return File(buffers, "image/jpeg");
            }
            catch
            {
                return File(new byte[0], "image/jpeg");
            }
        }

        /// <summary>
        /// 切换数据库的 webapi.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeDataBase([FromForm] string kind)
        {
            dbCollection.SetActive(kind);
            return Json(new WebApiResult<object> { code = ResultCode.STATE_OK, message = string.Empty });
        }
    }
}
