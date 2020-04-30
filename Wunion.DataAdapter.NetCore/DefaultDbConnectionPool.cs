using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 表示数据库连接池对象.
    /// </summary>
    public class DefaultDbConnectionPool : IDbConnectionPool
    {
        /// <summary>
        /// 空闲连接池.
        /// </summary>
        private List<ConnectionPoolItem> IdlePool;
        /// <summary>
        /// 正在占用的连接池.
        /// </summary>
        private List<ConnectionPoolItem> usingPool;
        private object poolLocked;
        private object forcedReleaseRunning;

        /// <summary>
        /// 创建一个 <see cref="DefaultDbConnectionPool"/> 的对象实列.
        /// </summary>
        internal DefaultDbConnectionPool()
        {
            IdlePool = new List<ConnectionPoolItem>();
            usingPool = new List<ConnectionPoolItem>();
            poolLocked = new object();
            forcedReleaseRunning = false;
        }

        /// <summary>
        /// 获取或设置请求连接的超时时间（超过该时间未获得连接分配则引发异常）.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// 自连接分配开始，在此时间后仍然未释放的连接进行强制回收.
        /// </summary>
        public TimeSpan ReleaseTimeout { get; set; }

        /// <summary>
        /// 获取或设置连接池的最大连接数.
        /// </summary>
        public int MaximumConnections { get; set; }

        /// <summary>
        /// 获取连接池中现有的连接数.
        /// </summary>
        public int Count => IdlePool.Count + usingPool.Count;

        /// <summary>
        /// 用于将给定的连接添加到连接池中，若连接池已满则引发异常.
        /// </summary>
        /// <param name="connection">要加入连接池的连接.</param>
        public void Add(IDbConnection connection)
        {
            if (Count >= MaximumConnections)
                throw new Exception("The connection pool is full. 连接池已满！");
            lock (poolLocked)
            {
                usingPool.Add(new ConnectionPoolItem {
                    Connection = connection,
                    LastUsed = DateTime.Now
                });
            }
        }

        /// <summary>
        /// 从连接池中获取一个数据库连接.
        /// </summary>
        /// <param name="makeFactory">当连接池为空时用于创建连接的方法.</param>
        /// <returns></returns>
        public IDbConnection GetConnection(MakeConnectionFactory makeFactory)
        {
            if (makeFactory == null)
                throw new ArgumentNullException(nameof(makeFactory));

            IDbConnection connection = null;
            if (Count < MaximumConnections && IdlePool.Count < 1) // 当连接池中无闲置连接，并且连接数未达上限时创建新的连接.
            {
                connection = makeFactory();
                Add(connection);
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                return connection;
            }
            DateTime timeMemory = DateTime.Now;
            ConnectionPoolItem poolItem = null;
            do
            {
                if (IdlePool.Count > 0)
                {
                    poolItem = IdlePool.First();
                    lock (poolLocked)
                    {
                        usingPool.Add(poolItem);
                        IdlePool.Remove(poolItem);
                    }
                    break;
                }
                Thread.Sleep(1);
            } while ((DateTime.Now - timeMemory).TotalSeconds < RequestTimeout.TotalSeconds);
            if (poolItem == null)
                throw new Exception("从数据库连接池中获取连接时超时. Timeout while getting connection from connection pool");
            poolItem.LastUsed = DateTime.Now;
            if (poolItem.Connection.State == ConnectionState.Closed)
                poolItem.Connection.Open();
            RunForcedRelease();
            return poolItem.Connection;
        }

        /// <summary>
        /// 用于收回指定的连接但不断开连接.
        /// </summary>
        /// <param name="connection">要收回的连接.</param>
        public void ReleaseConnection(IDbConnection connection)
        {
            if (connection == null)
                return;
            lock (poolLocked)
            {
                IEnumerable<ConnectionPoolItem> items = usingPool.Where(p => Object.ReferenceEquals(connection, p.Connection));
                if (items != null && items.Count() > 0)
                {
                    ConnectionPoolItem item = items.First();
                    usingPool.Remove(item);
                    IdlePool.Add(item);
                }
            }
            RunForcedRelease();
        }

        /// <summary>
        /// 运行连接的强制回收任务.
        /// </summary>
        private void RunForcedRelease()
        {
            if ((bool)forcedReleaseRunning)
                return;
            Interlocked.Exchange(ref forcedReleaseRunning, true);
            Task.Run(() => {
                try
                {
                    ConnectionPoolItem item;
                    for (int i = 0; i < usingPool.Count; ++i)
                    {
                        item = usingPool[i];
                        if ((DateTime.Now - item.LastUsed).TotalMinutes < ReleaseTimeout.TotalMinutes)
                            continue; // 占用连接未到超时强制释放时间.
                        // 回收超时未释放连接.
                        lock (poolLocked)
                        {
                            usingPool.RemoveAt(i--);
                            item.Connection.Close();
                            IdlePool.Add(item);
                        }
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref forcedReleaseRunning, false);
                }
            });
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
        }
    }
}
