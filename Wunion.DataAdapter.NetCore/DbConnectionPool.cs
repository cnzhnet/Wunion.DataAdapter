using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.DbInterop;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 用于规范华数据库连接池的接口.
    /// </summary>
    public interface IDbConnectionPool : IDisposable
    { 
        /// <summary>
        /// 获取或设置请求连接的超时时间（超过该时间未获得连接分配则引发异常）.
        /// </summary>
        DateTimeOffset RequestTimeout { get; set; }

        /// <summary>
        /// 自连接分配开始，在此时间后仍然未释放的连接进行强制回收.
        /// </summary>
        DateTimeOffset ReleaseTimeout { get; set; }

        /// <summary>
        /// 获取或设置连接池的最大连接数.
        /// </summary>
        int MaximumConnections { get; set; }

        /// <summary>
        /// 获取连接池中现有的连接数.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 用于将给定的连接添加到连接池中，若连接池已满则引发异常.
        /// </summary>
        /// <param name="connection">要加入连接池的连接.</param>
        void Add(IDbConnection connection);

        /// <summary>
        /// 从连接池中获取一个数据库连接.
        /// </summary>
        /// <param name="dba">当连接池为空时用于创建连接的数据访问器对象.</param>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// 用于收回指定的连接但不断开连接.
        /// </summary>
        /// <param name="connection">要收回的连接.</param>
        void ReleaseConnection(IDbConnection connection);
    }

    /// <summary>
    /// 表示数据库连接池对象.
    /// </summary>
    public class DefaultDbConnectionPool : IDbConnectionPool
    {
        private List<ConnectionPoolItem> pool;

        /// <summary>
        /// 创建一个 <see cref="DefaultDbConnectionPool"/> 的对象实列.
        /// </summary>
        internal DefaultDbConnectionPool()
        {
            pool = new List<ConnectionPoolItem>();
        }

        /// <summary>
        /// 获取或设置请求连接的超时时间（超过该时间未获得连接分配则引发异常）.
        /// </summary>
        public DateTimeOffset RequestTimeout { get; set; }

        /// <summary>
        /// 自连接分配开始，在此时间后仍然未释放的连接进行强制回收.
        /// </summary>
        public DateTimeOffset ReleaseTimeout { get; set; }

        /// <summary>
        /// 获取或设置连接池的最大连接数.
        /// </summary>
        public int MaximumConnections { get; set; }

        /// <summary>
        /// 获取连接池中现有的连接数.
        /// </summary>
        public int Count => pool.Count;

        /// <summary>
        /// 用于将给定的连接添加到连接池中，若连接池已满则引发异常.
        /// </summary>
        /// <param name="connection">要加入连接池的连接.</param>
        public void Add(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 从连接池中获取一个数据库连接.
        /// </summary>
        /// <param name="dba">当连接池为空时用于创建连接的数据访问器对象.</param>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 用于收回指定的连接但不断开连接.
        /// </summary>
        /// <param name="connection">要收回的连接.</param>
        public void ReleaseConnection(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        #region IDisposable成员
        /// <summary>
        /// 释放连接池占用的资源.
        /// </summary>
        /// <param name="disposing">手动释放则为 true，否则为 false.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            { }
        }

        /// <summary>
        /// 释放连接池占用的资源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构函数.
        /// </summary>
        ~DefaultDbConnectionPool()
        {
            Dispose(false);
        }
        #endregion

        /// <summary>
        /// 表示连接池中的连接项.
        /// </summary>
        internal class ConnectionPoolItem
        {
            /// <summary>
            /// 创建一个 <see cref="ConnectionPoolItem"/> 的对象实例.
            /// </summary>
            internal ConnectionPoolItem()
            { }

            /// <summary>
            /// 该连接的最后一次使用时间.
            /// </summary>
            internal DateTime LastUsed { get; set; }

            /// <summary>
            /// 数据库连接对象.
            /// </summary>
            internal IDbConnection Connection { get; set; }

            /// <summary>
            /// 该连接是否处于闲置状态.
            /// </summary>
            internal bool Idle { get; set; }
        }
    }
}
