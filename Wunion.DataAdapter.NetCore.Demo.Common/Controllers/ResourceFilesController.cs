using System.Text;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Wunion.DataAdapter.NetCore.Demo.Controllers
{
    /// <summary>
    /// 用于获取资源文件的类
    /// </summary>
    public class ResourceFilesController : BaseController
    {
        public ResourceFilesController(IHostingEnvironment hosting) : base(hosting)
        { }

        /// <summary>
        /// 获得指定的资源文件。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult File(string id)
        {
            if (FilesCache == null)
                FilesCache = new Dictionary<string, object>();
            string key = Path.GetFileNameWithoutExtension(id);
            key = key.Replace(".", "_").Replace("-", "_");
            object FileContent = null;
            // 首先从缓存中查找资源文件，若未缓存则从 dll 内嵌资源中获取。
            if (FilesCache.ContainsKey(key))
                FileContent = FilesCache[key];
            else
                FileContent = global::Wunion.DataAdapter.NetCore.Demo.Properties.Resources.ResourceManager.GetObject(key);
            string contentType = GetContentType(Path.GetExtension(id).ToLower());
            if (FileContent == null)
                return new FileStreamResult(new MemoryStream(), contentType);
            // 检查是否启用了资源文件缓存，若启用则将缓存中没有的文件加入缓存中。
            if (EnabledMemoryCached && !(FilesCache.ContainsKey(key)))
                FilesCache.Add(key, FileContent);
            if (FileContent is string)
            {
                ContentResult ContentView = new ContentResult();
                ContentView.ContentType = string.Format("{0};charset=UTF-8", contentType);
                ContentView.Content = FileContent.ToString();
                return ContentView;
            }
            else
            {
                return new FileStreamResult(new MemoryStream((byte[])FileContent), contentType);
            }
        }

        #region 静态成员

        private static Dictionary<string, string> HttpContentTypes;
        protected static Dictionary<string, object> FilesCache;

        /// <summary>
        /// 获取或设置是否启用资源文件的内存缓存。
        /// </summary>
        public static bool EnabledMemoryCached { get; set; }

        /// <summary>
        /// 初始化支持的资源文件 Content-Type 类型。
        /// </summary>
        public static void InitializeHttpContentTypes()
        {
            if (HttpContentTypes == null)
                HttpContentTypes = new Dictionary<string, string>();
            HttpContentTypes.Clear();
            HttpContentTypes.Add(".js", "application/javascript");
            HttpContentTypes.Add(".css", "text/css");
            HttpContentTypes.Add(".html", "text/html");
            HttpContentTypes.Add(".txt", "text/plain");
            HttpContentTypes.Add(".xml", "application/xml");
            HttpContentTypes.Add(".json", "application/json");
            HttpContentTypes.Add(".bmp", "application/x-bmp");
            HttpContentTypes.Add(".gif", "image/gif");
            HttpContentTypes.Add(".ico", "image/x-icon");
            HttpContentTypes.Add(".jpeg", "image/jpeg");
            HttpContentTypes.Add(".jpg", "image/jpeg");
            HttpContentTypes.Add(".png", "image/png");
            HttpContentTypes.Add(".tif", "image/tiff");
            HttpContentTypes.Add(".tiff", "image/tiff");
            HttpContentTypes.Add(".mid", "audio/mid");
            HttpContentTypes.Add(".mp3", "audio/mp3");
            HttpContentTypes.Add(".avi", "video/av");
            HttpContentTypes.Add(".mp4", "video/mpeg4");
            HttpContentTypes.Add(".mpeg", "video/mpg");
        }

        /// <summary>
        /// 获取资源文件的 Mime-Type 类型。
        /// </summary>
        /// <param name="ExtensionName">文件扩展名。</param>
        /// <returns></returns>
        protected static string GetContentType(string ExtensionName)
        {
            if (HttpContentTypes.ContainsKey(ExtensionName))
                return HttpContentTypes[ExtensionName];
            else
                return "application/octet-stream";
        }

        #endregion
    }
}
