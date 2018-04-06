#if NET462
using System;
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting;

namespace Wunion.DataAdapter.NetCore.Demo
{
    public static class WebHostExtensions
    {
        /// <summary>
        /// 为 IWebHost 类型的对象实例扩展一个 RunAsWindowsService 方法。
        /// </summary>
        /// <param name="host">对象实例。</param>
        public static void RunAsWindowsService(this IWebHost host)
        {
            ServiceBase webHostService = new CustomWebHostService(host);
            ServiceBase.Run(webHostService);
        }

        public static int IndexOf(this string[] array, string item)
        {
            return Array.IndexOf<string>(array, item);
        }
    }
}
#endif
