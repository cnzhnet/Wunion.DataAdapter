using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 用于创建数据库连接对象的工厂方法委托.
    /// </summary>
    /// <returns></returns>
    public delegate IDbConnection MakeConnectionFactory();

    /// <summary>
    /// 用于规范华数据库连接池的接口.
    /// </summary>
    public interface IDbConnectionPool : IDisposable
    {
        /// <summary>
        /// 获取或设置请求连接的超时时间（超过该时间未获得连接分配则引发异常）.
        /// </summary>
        TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// 自连接分配开始，在此时间后仍然未释放的连接进行强制回收.
        /// </summary>
        TimeSpan ReleaseTimeout { get; set; }

        /// <summary>
        /// 获取或设置连接池的最大连接数.
        /// </summary>
        int MaximumConnections { get; set; }

        /// <summary>
        /// 获取连接池中现有的连接数.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 从连接池中获取一个数据库连接.
        /// </summary>
        /// <param name="makeFactory">当连接池为空时用于创建连接的方法.</param>
        /// <returns></returns>
        IDbConnection GetConnection(MakeConnectionFactory makeFactory);

        /// <summary>
        /// 用于收回指定的连接但不断开连接.
        /// </summary>
        /// <param name="connection">要收回的连接.</param>
        void ReleaseConnection(IDbConnection connection);
    }
}
