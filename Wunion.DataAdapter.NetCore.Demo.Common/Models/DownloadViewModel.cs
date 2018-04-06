using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Wunion.DataAdapter.NetCore.Demo.Models
{
    public class DownloadViewModel
    {
        private HttpContext context;

        /// <summary>
        /// 获得客户端 IP 地址。
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 获得 netstandard 平台源码下载次数。
        /// </summary>
        public long NetstandardCount { get; set; }

        /// <summary>
        /// 获得 .NET Framework 4.x 平台源码下载次数。
        /// </summary>
        public long Net4xCount { get; set; }

        public DownloadViewModel(HttpContext httpContext)
        {
            context = httpContext;
            ClientIP = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

            Dictionary<string, Dictionary<string, object>> data = GetDownloadInfo();
            if (data.ContainsKey("netstandard"))
                NetstandardCount = Convert.ToInt64(data["netstandard"]["count"]);
            if (data.ContainsKey("net4x"))
                Net4xCount = Convert.ToInt64(data["net4x"]["count"]);
        }

        /// <summary>
        /// 读取更新日志信息。
        /// </summary>
        /// <returns></returns>
        public string ReadUpdateLog()
        {
            string logFile = string.Format(@"{0}\wwwroot\Downloads\update_log.html", AppServices.ContentRoot);
            if (!(System.IO.File.Exists(logFile)))
                return string.Empty;
            string context = string.Empty;
            using (System.IO.TextReader Rd = new System.IO.StreamReader(logFile, Encoding.UTF8))
            {
                context = Rd.ReadToEnd();
            }
            return context;
        }

        /// <summary>
        /// 获取下载数量
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, object>> GetDownloadInfo()
        {
            string WebRoot = string.Format(@"{0}\wwwroot", AppServices.ContentRoot);
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(WebRoot);
            builder.AddJsonFile("source.download.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, object> item;
            IConfigurationSection section = configuration.GetSection("netstandard");
            if (section != null)
            {
                item = new Dictionary<string, object>();
                item.Add("file", section.GetValue<string>("file"));
                item.Add("count", section.GetValue<int>("count"));
                data.Add("netstandard", item);
            }
            section = configuration.GetSection("net4x");
            if (section != null)
            {
                item = new Dictionary<string, object>();
                item.Add("file", section.GetValue<string>("file"));
                item.Add("count", section.GetValue<int>("count"));
                data.Add("net4x", item);
            }
            return data;
        }

        /// <summary>
        /// 计算并保存下载次数。
        /// </summary>
        /// <param name="key"></param>
        public static void CalcDownloadCount(string key)
        {
            try
            {
                string RecordFile = string.Format(@"{0}\wwwroot\source.download.json", AppServices.ContentRoot);
                Dictionary<string, Dictionary<string, object>> data = GetDownloadInfo();
                long Val = Convert.ToInt64(data[key]["count"]);
                data[key]["count"] = Val + 1;
                byte[] buff = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented));
                using (System.IO.FileStream fs = System.IO.File.Open(RecordFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                {
                    fs.Write(buff, 0x0, buff.Length);
                    fs.Flush();
                }
            }
            catch { }
        }
    }
}
