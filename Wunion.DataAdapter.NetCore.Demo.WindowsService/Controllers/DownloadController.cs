using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Wunion.DataAdapter.NetCore.Demo.Models;

namespace Wunion.DataAdapter.NetCore.Demo.Controllers
{
    public class DownloadController : BaseController
    {
        public DownloadController(IHostingEnvironment hosting) : base(hosting)
        { }

        public IActionResult Content()
        {
            return View(new DownloadViewModel(HttpContext));
        }

        private IActionResult ErrorView()
        {
            ContentResult ResultView = new ContentResult();
            ResultView.Content = "<h4>很抱歉，由于某些原因，该文件暂不提供下载！</h4>";
            ResultView.ContentType = "text/html;charset=UTF-8";
            return ResultView;
        }

        public IActionResult SourceCode()
        {
            string Platform = HttpContext.Request.Query["platform"];
            Dictionary<string, Dictionary<string, object>> data = DownloadViewModel.GetDownloadInfo();
            if (!data.ContainsKey(Platform))
                return ErrorView();
            string fileName = (data[Platform].ContainsKey("file")) ? data[Platform]["file"].ToString() : null;
            if (string.IsNullOrEmpty(fileName))
                return ErrorView();
            // 计算下载次数。
            DownloadViewModel.CalcDownloadCount(Platform);
            string FullPath = System.IO.Path.Combine(string.Format(@"{0}\Downloads", HostingEnvironment.WebRootPath), fileName);
            if (!(System.IO.File.Exists(FullPath)))
                return ErrorView();
            FileStreamResult FileView = new FileStreamResult(System.IO.File.Open(FullPath, FileMode.Open, FileAccess.Read), "application/octet-stream");
            FileView.FileDownloadName = fileName;
            return FileView;
        }
    }
}
