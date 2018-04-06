using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wunion.DataAdapter.NetCore.Demo.Controllers;

namespace Wunion.DataAdapter.NetCore.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BaseController.Configuration = Configuration; // 添加到控制器（便于在控制器中获取配置文件）

            ResourceFilesController.EnabledMemoryCached = Configuration.GetValue<bool>("ResourceFilesMemoryCached");
            ResourceFilesController.InitializeHttpContentTypes();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // 添加 Session 支持（需要手动添加NuGet包：Microsoft.AspNetCore.Session）
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
                // 扩展 Url 路由
                routes.MapRoute(
                    name: "download",
                    template: "Download",
                    defaults: new { controller = "Download", action = "Content" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
