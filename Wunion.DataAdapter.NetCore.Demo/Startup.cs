using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.SQLite3;
using Wunion.DataAdapter.Kernel.SQLite3.CommandParser;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.Kernel.SQLServer.CommandParser;
using Wunion.DataAdapter.NetCore.Demo.Controllers;

namespace Wunion.DataAdapter.NetCore.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BaseController.Configuration = Configuration; // 添加到控制器（便于在控制器中获取配置文件）
            ResourceFilesController.InitializeHttpContentTypes();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // 添加 Session 支持
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSession(); // 启用 Session 支持。

            // 从 appsettings.json 初始化数据引擎。
            AppServices.UseDataEngine(env, Configuration);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
