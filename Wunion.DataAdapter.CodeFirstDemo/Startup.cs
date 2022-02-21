using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Wunion.DataAdapter.CodeFirstDemo.Data;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// 获取指定类型数据库的设置段.
        /// </summary>
        /// <param name="kind">数据库类型.</param>
        /// <returns></returns>
        private IConfigurationSection GetDbSettings(string kind)
        {
            IConfigurationSection section = Configuration.GetSection("Database").GetSection("Default");
            return section.GetSection(kind);
        }

        private void SetDbOptionsWithPool(DbEngineConfiguration c)
        {
            IConfigurationSection section = GetDbSettings(c.Kind);
            c.DbEngine.DBA.ConnectionString = section.GetValue<string>("ConnectionString");
            section = section.GetSection("ConnectionPool");
            int maxConnections = section.GetValue<int>("MaxConnections");
            if (maxConnections > 0)
            {
                c.DbEngine.UseDefaultConnectionPool((pool) => {
                    pool.RequestTimeout = TimeSpan.FromSeconds(section.GetValue<double>("RequestTimeout"));
                    pool.ReleaseTimeout = TimeSpan.FromMinutes(section.GetValue<double>("ReleaseTimeout"));
                    pool.MaximumConnections = maxConnections;
                });
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions((options) => {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wunion.DataAdapter.CodeFirstDemo", Version = "v1" });
                string basePath = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                c.IncludeXmlComments(System.IO.Path.Combine(basePath, "Wunion.DataAdapter.CodeFirstDemo.xml"));
            });
            services.AddDbContainer((container) => {
                container.DbKind = "sqlite3";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wunion.DataAdapter.CodeFirstDemo v1"));
            }

            // 数据库配置.
            app.UseSqlServer((c) => SetDbOptionsWithPool(c));
            app.UseMySql((c) => SetDbOptionsWithPool(c));
            app.UseNpgsql((c) => SetDbOptionsWithPool(c));
            app.UseSqlite3((c) => {
                IConfigurationSection section = GetDbSettings(c.Kind);
                c.DbEngine.DBA.ConnectionString = section.GetValue<string>("ConnectionString");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
