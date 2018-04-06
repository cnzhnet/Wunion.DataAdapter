using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Wunion.DataAdapter.NetCore.Demo
{
    public class Program
    {
        private static IWebHost MainWebHost;

        /// <summary>
        /// 服务的入口函数。
        /// </summary>
        /// <param name="args">启动服务的命令行参数。</param>
        public static void Main(string[] args)
        {
            // 默认情况下，该服务以 Windows 服务的方式运行，使用 --NotWindowsService 命令行参数可让其以非 Windows 服务的方式运行。
            List<string> Argments = new List<string>(args);
            int RunAsServiceIndex = Argments.IndexOf("--NotWindowsService");
            if (RunAsServiceIndex != -1)
                Argments.RemoveAt(RunAsServiceIndex); // 在自定义的命令行参数判断完成后应删除它（否则程序会出错）。
            IConfiguration AppSettings = BuildConfiguration();
            MainWebHost = BuildWebHost(Argments.ToArray(), AppSettings);
            AppServices.ContentRoot = ContentRoot;
#if NET462
            if (RunAsServiceIndex != -1)
                MainWebHost.Run();
            else
                MainWebHost.RunAsWindowsService();
#else
            MainWebHost.Run();
#endif
        }

        /// <summary>
        /// 创建 WebHost 对象。
        /// </summary>
        /// <param name="args">启动服务时的命令行参数。</param>
        /// <param name="appSettings">appsettings.json 配置文件的读取器对象。</param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args, IConfiguration appSettings)
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            IConfigurationSection Section = appSettings.GetSection("BindingUrls");
            if (Section != null)
            {
                if (Section.GetValue<bool>("Enabled"))
                {
                    string Urls = Section.GetValue<string>("Urls");
                    string[] UrlArray = Urls.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    builder.UseUrls(UrlArray);
                }
            }
            builder.UseKestrel();
            builder.UseContentRoot(ContentRoot);
            builder.UseStartup<Startup>();
            return builder.Build();
        }

        private static string ContentRoot;

        /// <summary>
        /// 查找 appsettings.json 配置文件，并创建它的读取器对象。
        /// </summary>
        /// <returns></returns>
        private static IConfiguration BuildConfiguration()
        {
            ContentRoot = AppDomain.CurrentDomain.BaseDirectory;
            // appsettings.json在根目录，但 IDE 调试时根目录可能不正确，下面用循环找到它。
            // 此代码亦不影响发布后的运行情况。
            DirectoryInfo DirInfo = new DirectoryInfo(ContentRoot);
            do
            {
                if (DirInfo.GetFiles("appsettings.json").Length > 0)
                {
                    ContentRoot = DirInfo.FullName;
                    break;
                }
                else
                {
                    DirInfo = DirInfo.Parent;
                }
            } while (DirInfo.Parent != null);
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(ContentRoot);
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }
}
