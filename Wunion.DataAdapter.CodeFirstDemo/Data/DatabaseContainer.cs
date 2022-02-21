using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Wunion.DataAdapter.Kernel.CodeFirst;

namespace Wunion.DataAdapter.CodeFirstDemo.Data
{
    /// <summary>
    /// 表示应用程序的数据库容器.
    /// </summary>
    public interface IDatabaseContainer
    { 
        /// <summary>
        /// 获取或设置当前活动的数据库种类.
        /// </summary>
        string DbKind { get; set; }

        /// <summary>
        /// 添加一个数据库引擎的配置信息.
        /// </summary>
        /// <param name="key">键名称.</param>
        /// <param name="configuration">配置信息.</param>
        void AddDbEngine(string key, DbEngineConfiguration configuration);

        /// <summary>
        /// 获取数据库上下文对象.
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext;
    }

    /// <summary>
    /// 表示数据引擎的配置信息.
    /// </summary>
    public sealed class DbEngineConfiguration
    {
        /// <summary>
        /// 创建一个 <see cref="DbEngineConfiguration"/> 的对象实例.
        /// </summary>
        public DbEngineConfiguration()
        { }

        /// <summary>
        /// 表示数据库种类.
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// 数据库引擎.
        /// </summary>
        public DataEngine DbEngine { get; set; }

        /// <summary>
        /// 设置用于创建数据库上下文对象实例的方法.
        /// </summary>
        public Func<DataEngine, DbContext> DbContextFactory { private get; set; }

        /// <summary>
        /// 创建数据库上下文对象.
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="engine">数据库引擎.</param>
        /// <returns></returns>
        public TDbContext CreateDbContext<TDbContext>(DataEngine engine) where TDbContext : DbContext
        {
            if (DbContextFactory == null)
                return null;
            return DbContextFactory(engine) as TDbContext;
        }
    }

    /// <summary>
    /// 数据库容器.
    /// </summary>
    public class DatabaseContainer : IDatabaseContainer
    {
        private ConcurrentDictionary<string, DbEngineConfiguration> dbEnginePool;

        /// <summary>
        /// 创建一个 <see cref="DatabaseContainer"/> 对象实例.
        /// </summary>
        public DatabaseContainer()
        {
            dbEnginePool = new ConcurrentDictionary<string, DbEngineConfiguration>();
        }

        /// <summary>
        /// 获取或设置当前活动的数据库种类.
        /// </summary>
        public string DbKind { get; set; }

        public void AddDbEngine(string key, DbEngineConfiguration configuration)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            dbEnginePool.TryAdd(key, configuration);
        }

        public TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {
            DbEngineConfiguration configuration;
            if (dbEnginePool.TryGetValue(DbKind, out configuration))
                return configuration.CreateDbContext<TDbContext>(configuration.DbEngine);
            throw new Exception($"No DbEngine configuration found with key name: {DbKind}");
        }
    }
}
