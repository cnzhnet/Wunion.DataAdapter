using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Wunion.DataAdapter.NetCore.Test
{
    /// <summary>
    /// 扩展 <see cref="HttpContext"/> 对象的方法.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// 获取临时目录.
        /// </summary>
        /// <param name="controller">控制器对象实例.</param>
        /// <param name="env">包含运行时环境变量的对象.</param>
        /// <returns></returns>
        public static string GetTemporaryPath(this Controller controller, IWebHostEnvironment env)
        {
            string tmpPath = System.IO.Path.Combine(env.ContentRootPath, ".tmp");
            if (!(System.IO.Directory.Exists(tmpPath)))
                System.IO.Directory.CreateDirectory(tmpPath);
            return tmpPath;
        }

        /// <summary>
        /// 从 Form body 中读取所有字节.
        /// </summary>
        /// <param name="controller">控制器对象.</param>
        /// <returns></returns>
        public static async Task<byte[]> ReadFormBodyAsync(this Controller controller)
        {
            if (controller.HttpContext.Request.ContentLength == null && controller.HttpContext.Request.ContentLength < 1)
                throw new Exception("未提交任何内容.");

            long contentLen = controller.HttpContext.Request.ContentLength.Value;
            byte[] buffer = new byte[contentLen];
            int readCompleted = 0;
            do
            {
                readCompleted += await controller.HttpContext.Request.Body.ReadAsync(buffer, readCompleted, buffer.Length - readCompleted);
            } while (readCompleted < buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 将 Form Body 内容按文本格式读取并返回.
        /// </summary>
        /// <param name="controller">控制器对象.</param>
        /// <param name="mimeTypes">用于输出请求中指定的数据格式的集合.</param>
        /// <returns></returns>
        public static async Task<string> ReadBodyStringAsync(this Controller controller, List<string> mimeTypes)
        {
            // 获取并分离客户端提交的 content-type 类型.
            string contentType = controller.HttpContext.Request.ContentType as string;
            if (!string.IsNullOrEmpty(contentType))
                mimeTypes.AddRange(contentType.ToLower().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            // 从客户端类型中查找是否附带字符集编码信息，若有则使用客户端指定的字符中编码来解码 Body 内容，否则按 utf-8 解码.
            IEnumerable<string> charsetEnumerable = mimeTypes.Where(p => p.Contains("charset"));
            Encoding charSet = Encoding.UTF8;
            if (charsetEnumerable != null && charsetEnumerable.Count() > 0) //找到客户端字符集编码，获取其编码名称并初始化其 Encoding 对象.
            {
                string encodingName = charsetEnumerable.First();
                encodingName = encodingName.Replace("charset", string.Empty).Replace("=", string.Empty).Trim();
                charSet = Encoding.GetEncoding(encodingName);
            }
            // 读取 Body 流的内容.
            byte[] buffer = await controller.ReadFormBodyAsync();
            return charSet.GetString(buffer); // 将Body内容解码为文本并返回. 
        }

        /// <summary>
        /// 将 Form Body 中的 json 数据读取为字典（不支持对象嵌套）.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="ValueConvert"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, object>> ReadJsonBodyToDictionary(this Controller controller, Func<string, JsonElement, object> ValueConvert)
        {
            if (ValueConvert == null)
                throw new ArgumentNullException(nameof(ValueConvert));
            List<string> mimeTypes = new List<string>();
            string bodyContent = await controller.ReadBodyStringAsync(mimeTypes);
            bool is_json = mimeTypes.Contains("application/json");
            if (!is_json)
                is_json = mimeTypes.Contains("text/json");
            if (!is_json)
                throw new Exception("不正确的请求类型 Content-Type ，应为 application/json 或 text/json");

            Dictionary<string, object> data = new Dictionary<string, object>();
            JsonDocument json = JsonDocument.Parse(bodyContent);
            JsonElement.ObjectEnumerator enumerator = json.RootElement.EnumerateObject();
            object elemValue = null;
            while (enumerator.MoveNext())
            {
                elemValue = ValueConvert(enumerator.Current.Name, enumerator.Current.Value);
                if (elemValue == null)
                    continue;
                data.Add(enumerator.Current.Name, elemValue);
            }
            return data;
        }
    }
}
