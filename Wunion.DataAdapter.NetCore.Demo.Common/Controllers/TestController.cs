using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.NetCore.Demo.Models;
using Wunion.DataAdapter.NetCore.Demo.Services;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.NetCore.Demo.Controllers
{
    public class TestController : BaseController
    {
        public TestController(IHostingEnvironment hosting) : base(hosting)
        { }

        /// <summary>
        /// 切换数据引擎。
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangeDataEngine()
        {
            if (HttpContext.Request.Method.ToUpper() != "POST")
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "无效的请求。" });
            if (!(HttpContext.Request.Form.ContainsKey("key")))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "未指定数据引擎的有效键名称。" });
            string Key = HttpContext.Request.Form["key"];
            int rate = DataEngine.ChangeEngine(Key);
            ResultMessage ResultMsg = new ResultMessage();
            if (rate == DataEngine.ENGINE_CHANGE_OK)
            {
                ResultMsg.ResultCode = WebAPIStatus.STATE_OK;
            }
            else
            {
                ResultMsg.ResultCode = rate;
                ResultMsg.Message = "数据引擎切换失败。";
            }
            return new JsonResult(ResultMsg);
        }

        /// <summary>
        /// 获取所有分组表的记录。
        /// </summary>
        /// <returns></returns>
        public IActionResult GetAllGroup()
        {
            SpeedDataTable dt = AppServices.GetService<DataGroupService>().GetAllGroup();
            if (dt == null)
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.DATABASE_EXCEPTION, Message = "获取分组信息错误" });
            ResultMessage ResultMsg = new ResultMessage();
            if (dt.Count < 1)
            {
                ResultMsg.ResultCode = WebAPIStatus.STATE_NO_DATA;
                ResultMsg.Message = "没有数据。";
            }
            else
            {
                ResultMsg.ResultCode = WebAPIStatus.STATE_OK;
                ResultMsg.DataFormat = "encode:base64;charset:utf-8;text/json";
                ResultMsg.Data = dt.SerializeJson();
                ResultMsg.Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(ResultMsg.Data));
            }
            return new JsonResult(ResultMsg);
        }

        /// <summary>
        /// 添加分组信息。
        /// </summary>
        /// <returns></returns>
        public IActionResult AddNewGroup()
        {
            Dictionary<string, object> data = GetPostData((string Key, string Val) => Val.ToString());
            if (data == null)
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "请求方式错误。" });
            if (!data.ContainsKey("Name"))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少 Name 参数。" });
            if (AppServices.GetService<DataGroupService>().AddGroup(data["Name"].ToString()))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_OK });
            else
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "添加分组失败了。" });
        }

        /// <summary>
        /// 获取指定分组下的数据。
        /// </summary>
        /// <returns></returns>
        public IActionResult GetDataByGroup()
        {
            Dictionary<string, object> data = GetPostData((string Key, string Value) => string.IsNullOrEmpty(Value) ? -1 : Convert.ToInt32(Value));
            if (data == null)
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "请求方式错误。" });
            if (!(data.ContainsKey("pageSize")))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少 pageSize 参数。" });
            if (!(data.ContainsKey("page")))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少 page 参数。" });
            if (!(data.ContainsKey("GID")))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少 GID 参数。" });
            try
            {
                int Total = AppServices.GetService<DataService>().GetTotalCount((int)data["GID"]);
                SpeedDataTable dt = AppServices.GetService<DataService>().GetGroupData((int)data["GID"], (int)data["pageSize"], (int)data["page"]);
                if (Total > 0 && dt.Count > 0)
                {
                    StringBuilder buff = new StringBuilder("{");
                    buff.AppendFormat("\"total\": {0}", Total);
                    buff.AppendFormat(", \"rows\": {0}", dt.SerializeJson());
                    buff.Append("}");
                    ResultMessage ResultMsg = new ResultMessage();
                    ResultMsg.ResultCode = WebAPIStatus.STATE_OK;
                    ResultMsg.DataFormat = "encode:base64;charset:utf-8;text/json";
                    ResultMsg.Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(buff.ToString()));
                    return new JsonResult(ResultMsg);
                }
                else
                {
                    return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_NO_DATA, Message = "没有数据。" });
                }
            }
            catch (Exception Ex)
            {
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.INTERNAL_SERVER_ERROR, Message = Ex.Message });
            }
        }

        /// <summary>
        /// 获取详细信息。
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public IActionResult GetDetails(int id)
        {
            Dictionary<string, object> data = AppServices.GetService<DataService>().GetDetails(id);
            if (data == null)
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.DATABASE_EXCEPTION, Message = "获取详细信息失败。" });
            ResultMessage ResultMsg = new ResultMessage();
            ResultMsg.ResultCode = WebAPIStatus.STATE_OK;
            ResultMsg.DataFormat = "encode:base64;charset:utf-8;text/json";
            ResultMsg.Data = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            ResultMsg.Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(ResultMsg.Data));
            return new JsonResult(ResultMsg);
        }

        /// <summary>
        /// 获取照片信息。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Photo(int id)
        {
            byte[] data = AppServices.GetService<DataService>().GetPhotoData(id);
            if (data == null)
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_NO_DATA, Message = "没有照片数据。" });
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0x0;
            return new FileStreamResult(ms, "image/png");
        }

        /// <summary>
        /// 添加数据。
        /// </summary>
        /// <returns></returns>
        public IActionResult AddTestData()
        {
            if (HttpContext.Request.Method.ToUpper() != "POST")
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "错误的请求方式。" });
            if (!HttpContext.Request.Form.ContainsKey("GroupId"))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少参数。" });
            if (!HttpContext.Request.Form.ContainsKey("TestName"))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少参数。" });
            if (!HttpContext.Request.Form.ContainsKey("TestAge"))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少参数。" });
            if (!HttpContext.Request.Form.ContainsKey("TestSex"))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少参数。" });
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("GroupId", Convert.ToInt32(HttpContext.Request.Form["GroupId"]));
                data.Add("TestName", HttpContext.Request.Form["TestName"].ToString());
                data.Add("TestAge", Convert.ToSingle(HttpContext.Request.Form["TestAge"]));
                data.Add("TestSex", HttpContext.Request.Form["TestSex"].ToString());
                byte[] Bin;
                if (HttpContext.Request.Form.Files.Count < 1)
                {
                    data.Add("TestPhoto", DBNull.Value);
                }
                else
                {
                    Microsoft.AspNetCore.Http.IFormFile file = HttpContext.Request.Form.Files[0]; // 只接收一个图片文件
                    Bin = new byte[file.Length];
                    using (System.IO.Stream sm = file.OpenReadStream())
                    {
                        int rlen = 0x0, offset = 0x0;
                        do
                        {
                            rlen = sm.Read(Bin, offset, Bin.Length - offset);
                            offset += rlen;
                        } while (offset < Bin.Length);
                    }
                    data.Add("TestPhoto", Bin);
                }
                if (AppServices.GetService<DataService>().SaveData(data))
                    return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_OK });
                else
                    return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "提交数据失败了。" });
            }
            catch (Exception Ex)
            {
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.INTERNAL_SERVER_ERROR, Message = Ex.Message });
            }
        }
        
        /// <summary>
        /// 删除指定的数据。
        /// </summary>
        /// <returns></returns>
        public IActionResult DeleteData()
        {
            if (HttpContext.Request.Method.ToUpper() != "POST")
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "请求方式错误。" });
            if (!HttpContext.Request.Form.ContainsKey("filter"))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "缺少参数 filter。" });
            string filterText = HttpContext.Request.Form["filter"];
            if (string.IsNullOrEmpty(filterText))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "空条件将不执行任何删除操作。" });

            string[] array = filterText.Split(';', ',');
            object[] Filter = new object[array.Length];
            for (int i = 0; i < array.Length; ++i)
                Filter[i] = Convert.ToInt32(array[i]);
            if (AppServices.GetService<DataService>().DeleteData(Filter))
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_OK });
            else
                return new JsonResult(new ResultMessage { ResultCode = WebAPIStatus.STATE_FAIL, Message = "删除失败。" });
        }
    }
}