using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 实现在一个连接上分批处理数据的批处理器.
    /// </summary>
    public sealed class DataBatchProcessor : IDisposable
    {
        private BatchCommander _commander;
        private DataEngine Engine;

        /// <summary>
        /// 创建一个 <see cref="DataBatchProcessor"/> 的对象实例.
        /// </summary>
        /// <param name="_engine">此批处理使用的数据库引擎实例.</param>
        internal DataBatchProcessor(DataEngine _engine)
        {
            Engine = _engine;
            _commander = new BatchCommander(_engine);
        }

        /// <summary>
        /// 获取指定的数据表上下文对象.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public TContext Table<TContext>() where TContext : TableMapper, new()
        {
            TContext context = new TContext();
            context.BatchProccesser = this;
            context.SetDataEngine(Engine, Engine);
            return context;
        }

        /// <summary>
        /// 执行指定的命令，并返回受影响记录数.
        /// </summary>
        /// <param name="command">要执行的命令.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbCommandBuilder command)
        {
            return Commander.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 用于指在同一连接上执行命令的执行器.
        /// </summary>
        internal BatchCommander Commander => _commander;

        #region IDisposable成员实现

        /// <summary>
        /// 释放对象所占用的资源.
        /// </summary>
        /// <param name="disposing">手动调用则为 true，由对象终结器调用时则为 false .</param>
        private void Dispose(bool disposing)
        {
            if (_commander != null)
                _commander.Dispose();
            _commander = null;
        }

        /// <summary>
        /// 释放对象占用的资源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 对象终结器（析构函数）.
        /// </summary>
        ~DataBatchProcessor()
        {
            Dispose(false);
        }
        #endregion
    }
}
