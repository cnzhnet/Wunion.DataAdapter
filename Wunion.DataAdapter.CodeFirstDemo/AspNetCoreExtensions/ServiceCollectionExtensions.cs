using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandParser;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Microsoft.AspNetCore.Builder;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    /// <summary>
    /// 用于扩展 <see cref="IServiceCollection"/> 对象的方法.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static IDatabaseContainer container = null;

        /// <summary>
        /// 添加数据库容器的依赖注入支持.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure">用于设置数据库容器</param>
        public static void AddDbContainer(this IServiceCollection services, Action<IDatabaseContainer> configure)
        {
            container = container = new DatabaseContainer();
            services.AddSingleton<IDatabaseContainer>(container);
            configure(container);
        }
    }

    /// <summary>
    /// 用于扩展 <see cref="IApplicationBuilder"/>
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 使用 Microsoft SQL Server 数据库.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configure">用于配置数据库.</param>
        public static void UseSqlServer(this IApplicationBuilder app, Action<DbEngineConfiguration> configure)
        {
            IDatabaseContainer container = app.ApplicationServices.GetService<IDatabaseContainer>();
            if (container == null)
                throw new Exception("Please invoke the IServiceCollection.AddDbContainer extension method first.");

            const string dbKind = "mssql";
            DbEngineConfiguration configuration = new DbEngineConfiguration { Kind = dbKind };
            configuration.DbEngine = new DataEngine(
                new Wunion.DataAdapter.Kernel.SQLServer.SqlServerDbAccess(),
                new Wunion.DataAdapter.Kernel.SQLServer.CommandParser.SqlServerParserAdapter()
            );
            configuration.DbContextFactory = (engine) => new MyDbContext(engine);
            configure?.Invoke(configuration);
            container.AddDbEngine(dbKind, configuration);
        }

        /// <summary>
        /// 使用 MySQL 数据库.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configure">用于配置数据库.</param>
        public static void UseMySql(this IApplicationBuilder app, Action<DbEngineConfiguration> configure)
        {
            IDatabaseContainer container = app.ApplicationServices.GetService<IDatabaseContainer>();
            if (container == null)
                throw new Exception("Please invoke the IServiceCollection.AddDbContainer extension method first.");

            const string dbKind = "mysql";
            DbEngineConfiguration configuration = new DbEngineConfiguration { Kind = dbKind };
            configuration.DbEngine = new DataEngine(
                new Wunion.DataAdapter.Kernel.MySQL.MySqlDBAccess(),
                new Wunion.DataAdapter.Kernel.MySQL.CommandParser.MySqlParserAdapter()
            );
            configuration.DbContextFactory = (engine) => new MyDbContext(engine);
            configure?.Invoke(configuration);
            container.AddDbEngine(dbKind, configuration);
        }

        /// <summary>
        /// 使用 PostgreSQL 数据库.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configure">用于配置数据库.</param>
        public static void UseNpgsql(this IApplicationBuilder app, Action<DbEngineConfiguration> configure)
        {
            IDatabaseContainer container = app.ApplicationServices.GetService<IDatabaseContainer>();
            if (container == null)
                throw new Exception("Please invoke the IServiceCollection.AddDbContainer extension method first.");

            const string dbKind = "npgsql";
            DbEngineConfiguration configuration = new DbEngineConfiguration { Kind = dbKind };
            configuration.DbEngine = new DataEngine(
                new Wunion.DataAdapter.Kernel.PostgreSQL.NpgsqlDbAccess(),
                new Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser.NpgsqlParserAdapter()
            );
            configuration.DbContextFactory = (engine) => new MyDbContext(engine);
            configure?.Invoke(configuration);
            container.AddDbEngine(dbKind, configuration);
        }

        /// <summary>
        /// 使用 Sqlite3 数据库.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configure">用于配置数据库.</param>
        public static void UseSqlite3(this IApplicationBuilder app, Action<DbEngineConfiguration> configure)
        {
            IDatabaseContainer container = app.ApplicationServices.GetService<IDatabaseContainer>();
            if (container == null)
                throw new Exception("Please invoke the IServiceCollection.AddDbContainer extension method first.");

            const string dbKind = "sqlite3";
            DbEngineConfiguration configuration = new DbEngineConfiguration { Kind = dbKind };
            configuration.DbEngine = new DataEngine(
                new Wunion.DataAdapter.Kernel.SQLite3.SqliteDbAccess(),
                new Wunion.DataAdapter.Kernel.SQLite3.CommandParser.SqliteParserAdapter()
            );
            configuration.DbContextFactory = (engine) => new MyDbContext(engine);
            configure?.Invoke(configuration);
            container.AddDbEngine(dbKind, configuration);
        }
    }
}
