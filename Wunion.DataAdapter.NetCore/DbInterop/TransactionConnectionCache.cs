using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Data;

namespace Wunion.DataAdapter.Kernel.DbInterop
{
    /// <summary>
    /// 用于事务的连接缓存，当有事务产生时才将 Connection 缓存到此处。事务结束释放相关连接。
    /// </summary>
    public class TransactionConnectionCache
    {
        private static ConcurrentDictionary<string, TransactionCacheItem> _cachePool;

        /// <summary>
        /// 数据库连接对象。
        /// </summary>
        internal static ConcurrentDictionary<string, TransactionCacheItem> CachePool => _cachePool;

        /// <summary>
        /// 将事务相关的注册到待释放缓冲区.
        /// </summary>
        /// <param name="OwnerId">事务的有效唯一标识.</param>
        /// <param name="dba">与该事务相关的数据访问器.</param>
        /// <param name="conn">创建事务的连接.</param>
        public static void RegisterValidTransaction(string OwnerId, DbAccess dba, IDbConnection conn)
        {
            if (_cachePool == null)
                _cachePool = new ConcurrentDictionary<string, TransactionCacheItem>();
            if (!CachePool.ContainsKey(OwnerId))
                CachePool.TryAdd(OwnerId, new TransactionCacheItem { DBA = dba, Connection = conn });
        }

        /// <summary>
        /// 清理有效事务注册。
        /// </summary>
        public static void ReleaseConnection(string OwnerId)
        {
            if (!CachePool.ContainsKey(OwnerId))
                return;

            TransactionCacheItem item = null;
            if (!CachePool.TryRemove(OwnerId, out item))
                return;
            if (item.DBA.ConnectionPool != null && item.DBA.ConnectionPool.MaximumConnections > 0)
            {
                item.DBA.ConnectionPool.ReleaseConnection(item.Connection);
            }
            else
            {
                item.Connection.Close();
                item.Connection.Dispose();
            }
        }
    }

    /// <summary>
    /// 表示事务连接关联缓存的元素.
    /// </summary>
    internal class TransactionCacheItem
    {
        /// <summary>
        /// 创建一个 <see cref="TransactionCacheItem"/> 的对象实例.
        /// </summary>
        internal TransactionCacheItem()
        { }

        /// <summary>
        /// 对应的数据访问器.
        /// </summary>
        internal DbAccess DBA { get; set; }

        /// <summary>
        /// 打开事务的连接.
        /// </summary>
        internal IDbConnection Connection { get; set; }
    }
}
