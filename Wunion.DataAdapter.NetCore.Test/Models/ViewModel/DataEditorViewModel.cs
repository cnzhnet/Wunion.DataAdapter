using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Html;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.NetCore.Test.Services;

namespace Wunion.DataAdapter.NetCore.Test.Models
{
    /// <summary>
    /// 数据编辑器子视图的视图模型对象.
    /// </summary>
    public class DataEditorViewModel : ModuleViewModel
    {
        private TestDataItemService service;
        private Dictionary<string, object> entity;
        private bool editMode;

        /// <summary>
        /// 创建一个 <see cref="DataEditorViewModel"/> 的对象实例.
        /// </summary>
        public DataEditorViewModel() {
            editMode = false;
            Error = string.Empty;
        }

        /// <summary>
        /// 获取页面处理时产生的错误信息.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// 若找到对应的数据实体则返回 true，否则返回 false .
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            try
            {
                DatabaseCollection dbCollection = Context.RequestServices.GetService(typeof(DatabaseCollection)) as DatabaseCollection;
                if (dbCollection == null)
                    throw new Exception("未配置应用程序的数据库信息.");
                service = DataService.Get<TestDataItemService>(dbCollection.Current);
                if (Context.Request.Query.ContainsKey("id"))
                {
                    int dataId = -1;
                    editMode = int.TryParse(Context.Request.Query["id"].ToString(), out dataId);
                    if (!editMode)
                        throw new Exception("给定的记录 id 不正确.");
                    entity = service.Details(dataId);
                    if (entity == null)
                        throw new Exception("未找到指定的记录，它可能已经被其它用户删除！");
                }
                return true;
            }
            catch (Exception Ex)
            {
                Error = Ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取实体数据实体中指定数据项的值.
        /// </summary>
        /// <param name="name">数据项名称.</param>
        /// <returns></returns>
        public string EntityItem(string name)
        {
            if (!editMode)
                return string.Empty;
            object buffer = null;
            if (entity.TryGetValue(name, out buffer))
                return (buffer == null || buffer == DBNull.Value) ? string.Empty : buffer.ToString();
            return string.Empty;
        }

        /// <summary>
        /// 将实体对象序列化为 base64 格式的 json 字符串.
        /// </summary>
        /// <returns></returns>
        public string EntityToBase64()
        {
            if (entity == null)
                return string.Empty;
            string json = JsonSerializer.Serialize<Dictionary<string, object>>(entity, new JsonSerializerOptions {
                PropertyNamingPolicy = null, 
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        /// <summary>
        /// 生成分组下拉选框.
        /// </summary>
        /// <returns></returns>
        public IHtmlContent RenderGroupDropdown()
        {
            HtmlContentBuilder builder = new HtmlContentBuilder();
            builder.AppendHtml("<select name=\"GroupId\" id=\"group-id\" required>");
            GroupDataService gservice = DataService.Get<GroupDataService>(service.db);
            List<dynamic> groups = gservice.Query();
            int? groupId = null;
            if (editMode)
                groupId = Convert.ToInt32(entity["GroupId"]);
            foreach (dynamic grp in groups) 
            {
                if (groupId != null && groupId.Value == Convert.ToInt32(grp.GroupId))
                {
                    builder.AppendHtml(string.Format("<option value=\"{0}\" selected=\"selected\">{1}</option>", grp.GroupId, grp.GroupName));
                    continue;
                }
                builder.AppendHtml(string.Format("<option value=\"{0}\">{1}</option>", grp.GroupId, grp.GroupName));
            }
            builder.AppendHtml("</select>");
            return builder;
        }
    }
}
