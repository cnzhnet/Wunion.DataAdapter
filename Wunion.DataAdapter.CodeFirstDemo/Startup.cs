using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Wunion.DataAdapter.CodeFirstDemo.Data.Security;

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
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DateTimeNullableConverter());
                options.JsonSerializerOptions.Converters.Add(new UserAccountStatusJsonConverter());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wunion.DataAdapter.CodeFirstDemo", Version = "v1" });
                // 增加 swagger 的 token 授权.
                OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme { 
                    Description = "请先调用 /UserAccount/LogIn 然后复制返回的 Token 粘至此处：",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                };
                OpenApiSecurityRequirement requirement = new OpenApiSecurityRequirement();
                requirement.Add(new OpenApiSecurityScheme { 
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "WebApiAuth" }
                }, new string[] { });
                c.AddSecurityDefinition("WebApiAuth", securityScheme);
                c.AddSecurityRequirement(requirement);

                // 设置注释文件的路径.
                string basePath = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                c.IncludeXmlComments(System.IO.Path.Combine(basePath, "Wunion.DataAdapter.CodeFirstDemo.xml"));
            });
            services.AddDbContainer((container) => {
                container.DbKind = "mysql";
            });
            services.AddDbConverterOptions((options) => {
                options.Add(typeof(UserAccountStatus), new UserAccountStatusConverter());
                options.Add(typeof(List<int>), new IntegerCollectionConverter());
            });
            services.AddSingleton<IDataProtection>(new RsaDataProtection());
            services.AddScoped<WebApiExceptionFilter>();
            services.AddScoped<AuthorizationAccessor>();
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
            app.UseRsaProtect((dp) => {
                string pk = null;
                using (System.IO.TextReader reader = new System.IO.StreamReader(System.IO.Path.Combine(env.ContentRootPath, "Configuration", "rsa.pk")))
                    pk = reader.ReadToEnd();
                dp.ImportKey(pk);
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
