using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Wunion.DataAdapter.Kernel.DbInterop
{
    /// <summary>
    /// 用于事务的连接缓存，当有事务产生时才将 Connection 缓存到此处。事务结束就应该清除。
    /// </summary>
    public class TransactionConnectionCache
    {
        private static Dictionary<string, IDbConnection> _DbConnections;

        /// <summary>
        /// 数据库连接对象。
        /// </summary>
        public static Dictionary<string, IDbConnection> DbConnections
        {
            get { return _DbConnections; }
        }

        private static string ValidTransactionOwner; // 有效事务的所有者。

        /// <summary>
        /// 注册有效事务。
        /// </summary>
        /// <param name="OwnerId">所有者Id。</param>
        public static void RegisterValidTransaction(string OwnerId, IDbConnection Conn)
        {
            if (_DbConnections == null)
                _DbConnections = new Dictionary<string, IDbConnection>();
            if (!DbConnections.ContainsKey(OwnerId))
                DbConnections.Add(OwnerId, Conn);
        }

        /// <summary>
        /// 清理有效事务注册。
        /// </summary>
        public static void ReleaseConnection(string OwnerId)
        {
            if (!DbConnections.ContainsKey(OwnerId))
                return;
            IDbConnection Conn = DbConnections[OwnerId];
            DbConnections.Remove(OwnerId);
            Conn.Close();
            Conn.Dispose();
        }
    }
}
