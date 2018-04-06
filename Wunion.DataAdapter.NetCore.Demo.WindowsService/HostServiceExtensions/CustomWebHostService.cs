#if NET462
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace Wunion.DataAdapter.NetCore.Demo
{
    /// <summary>
    /// 扩展的自定义 WebHostService 服务。
    /// </summary>
    public class CustomWebHostService : WebHostService
    {
        public CustomWebHostService(IWebHost host) : base(host)
        {
            //
        }

        protected override void OnStarting(string[] args)
        {
            // 服务正在启动时要记录的日志。
        }

        protected override void OnStarted()
        {
            // 服务启动后要记录的日志。
        }

        protected override void OnStopping()
        {
            // 正在停止服务时要记录的日志。
        }

        protected override void OnStopped()
        {
            // 服务停止后要记录的日志。
        }
    }
}
#endif