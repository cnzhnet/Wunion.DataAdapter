using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Features;
using Wunion.DataAdapter.Kernel;

namespace Wunion.DataAdapter.NetCore.Test
{
    public class Startup
    {
        internal IConfiguration Configuration { get; set; }
        private IWebHostEnvironment hostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.Configuration = configuration;
            this.hostEnvironment = env;
        }

        /// <summary>
        /// 配置数据库.
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureDatabase(IServiceCollection services)
        {
            DatabaseCollection database = new DatabaseCollection();
            IConfigurationSection section = Configuration.GetSection("Database").GetSection("SQLServer");
            database.UseSqlserver(section.GetValue<string>("ConnectionString"), section.GetValue<int>("ConnectionPool", 0));
            
            section = Configuration.GetSection("Database").GetSection("MySQL");
            database.UseMySql(section.GetValue<string>("ConnectionString"), section.GetValue<int>("ConnectionPool", 0));

            section = Configuration.GetSection("Database").GetSection("PostgreSQL");
            database.UsePostgreSQL(section.GetValue<string>("ConnectionString"), section.GetValue<int>("ConnectionPool", 0));

            section = Configuration.GetSection("Database").GetSection("SQLite3");
            string sqliteConnectionString = section.GetValue<string>("ConnectionString");
            sqliteConnectionString = sqliteConnectionString.Replace("{contentroot}", hostEnvironment.ContentRootPath);
            database.UseSQLite3(sqliteConnectionString);
            database.SetActive("sqlite3");

            services.AddSingleton<DatabaseCollection>(database);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc((option) => {
                option.EnableEndpointRouting = false;
            }).AddJsonOptions((options) => {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DateTimeNullableConverter());
            });
            // 配置反向代理转接，以确保获得的客户端地址信息的正确性.
            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                // 设置允许受信任的代理网络转接头（防止受到 IP 欺骗攻击）。
                IConfigurationSection section = Configuration.GetSection("ForwardedHeadersOptions").GetSection("KnownProxies");
                KeyValuePair<string, string>[] array = section.AsEnumerable().ToArray();
                if (array != null && array.Length > 0)
                {
                    foreach (KeyValuePair<string, string> item in array)
                    {
                        if (string.IsNullOrEmpty(item.Value))
                            continue;
                        options.KnownProxies.Add(System.Net.IPAddress.Parse(item.Value));
                    }
                }
            });
            // 配置 http POST 最大上传限制
            services.Configure<FormOptions>(options => {
                long lengthLimit = 1024000000;
                IConfigurationSection frmOptions = Configuration.GetSection("FormOptions");
                if (frmOptions != null)
                    lengthLimit = frmOptions.GetValue<long>("MultipartBodyLengthLimit");
                options.MultipartBodyLengthLimit = lengthLimit;
            });
            services.AddSession((configure) => {
                configure.IdleTimeout = TimeSpan.FromMinutes(45);
                configure.IOTimeout = TimeSpan.FromMinutes(45);
            });
            services.AddHttpContextAccessor();
            services.AddSingleton(typeof(HtmlEncoder), HtmlEncoder.Create(UnicodeRanges.All)); // cshtml 视图的 Unicode 编码处理.
            services.AddScoped<WebApiExceptionFilter>();
            ConfigureDatabase(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders();
            app.UseStaticFiles();            
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRouting();
            app.UseMvc();
        }
    }
}
