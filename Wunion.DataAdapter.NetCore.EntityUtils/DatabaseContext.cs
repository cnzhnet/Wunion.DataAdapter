using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 支持读写分离模式的数据库上下文对象类型。
    /// </summary>
    public class DatabaseContext
    {
        /// <summary>
        /// 获取或设置用于读取的数据引擎.
        /// </summary>
        protected DataEngine ReadEngine { get; set; }

        /// <summary>
        /// 获取或设置用于写入的数据引擎.
        /// </summary>
        protected DataEngine WriteEngine { get; set; }

        /// <summary>
        /// 创建一个 <see cref="DatabaseContext"/> 的对象实例.
        /// </summary>
        protected DatabaseContext() { }

        /// <summary>
        /// 使用数据库连接池.
        /// </summary>
        /// <param name="configure">用于实现数据库连接池配置的方法.</param>
        public void UseConnectionPool(ConnectionPoolConfigureHandler configure)
        {
            configure(ReadEngine, WriteEngine);
        }

        /// <summary>
        /// 获取指定的数据表上下文对象.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public TContext Table<TContext>() where TContext : TableMapper, new()
        {
            TContext context = new TContext();
            context.SetDataEngine(ReadEngine, WriteEngine);
            return context;
        }

        /// <summary>
        /// 返回一个多表查询筛选器.
        /// </summary>
        /// <typeparam name="TEntityAgent">实体的查询代理类型.</typeparam>
        /// <returns></returns>
        public MultiQuerySelector From<TEntityAgent>() where TEntityAgent : EntityAgent, new()
        {
            MultiQuerySelector multiQuery = new MultiQuerySelector(ReadEngine);
            multiQuery.From<TEntityAgent>();
            return multiQuery;
        }

        /// <summary>
        /// 在写访问引擎上执行指定的命令并返回受影响记录数.
        /// </summary>
        /// <param name="Command">要执行的命令.</param>
        /// <param name="trans">在该事务中执行.</param>
        /// <exception cref="Exception">当命执行命令过程产生错误时引发该异常.</exception>
        /// <returns></returns>
        public int ExecuteNoneQuery(DbCommandBuilder Command, DBTransactionController trans = null)
        {
            int result = 0;
            if (trans == null)
            {
                result = WriteEngine.ExecuteNoneQuery(Command);
                if (result < 0 && WriteEngine.DBA.Error != null)
                    throw new Exception(WriteEngine.DBA.Error.Message);                
            }
            else
            {
                trans.DBA.Errors.Clear();
                result = trans.DBA.ExecuteNoneQuery(Command);
                if (result < 0 && trans.DBA.Errors.Count > 0)
                    throw new Exception(trans.DBA.Errors[0].Message);
            }
            return result;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="command">执行该构建器构建的命令.</param>
        /// <returns></returns>
        public object ExecuteScalar(DbCommandBuilder command)
        {
            object result = ReadEngine.ExecuteScalar(command);
            if (result == null && ReadEngine.DBA.Error != null)
                throw new Exception(ReadEngine.DBA.Error.Message);
            return result;
        }

        /// <summary>
        /// 执行指定的查询命令，并返回相应的数据读取器。
        /// </summary>
        /// <param name="Command">执行该构建器构建的查询命令.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(DbCommandBuilder Command)
        {
            IDataReader Rd = ReadEngine.ExecuteReader(Command);
            if (Rd == null && ReadEngine.DBA.Error != null)
                throw new Exception(ReadEngine.DBA.Error.Message);
            return Rd;
        }

        /// <summary>
        /// 开启并返回一个写数据的事务.
        /// <param name="li">事务锁定行为（即隔离级别）.</param>
        /// </summary>
        /// <returns>返回事务控制器对象.</returns>
        public DBTransactionController BeginTransAction(IsolationLevel? il = null)
        {
            return WriteEngine.BeginTrans(il);
        }

        /// <summary>
        /// 创建并返回一个用于在同一个连接上分批处理数据的处理器对象.
        /// </summary>
        /// <returns>返回一个用于在同一个连接上分批处理数据的处理器对象.</returns>
        public DataBatchProcessor BatchProcess()
        {
            return new DataBatchProcessor(WriteEngine);
        }

        /// <summary>
        /// 初始化并返加一个读写分离的数据库上下文对象.
        /// </summary>
        /// <param name="func">用于配置并返回需要的选项.</param>
        /// <returns></returns>
        public static DatabaseContext Initialize(Func<ContextOptions> func)
        {
            ContextOptions options = func();
            if (options == null)
                throw new NoNullAllowedException("The options required to initialize a database context object are not configured.");
            return Initialize(options);
        }

        /// <summary>
        /// 初始化并返加一个读写分离的数据库上下文对象.
        /// </summary>
        /// <param name="options">初始化数据库上下文对象所需要的选项.</param>
        /// <returns></returns>
        public static DatabaseContext Initialize(ContextOptions options)
        {
            DatabaseContext DbContext = new DatabaseContext();
            DbContext.ReadEngine = new DataEngine(options.ReadAccess, options.Parser);
            DbContext.WriteEngine = new DataEngine(options.WriteAccess, options.Parser);
            return DbContext;
        }

        /// <summary>
        /// 用于初始化数据库上下文对象的选项.
        /// </summary>
        public sealed class ContextOptions
        {
            /// <summary>
            /// 获取或设置用于读取访问的数据访问器.
            /// </summary>
            public DbAccess ReadAccess { get; set; }

            /// <summary>
            /// 获取或设置写访问的数据访问器.
            /// </summary>
            public DbAccess WriteAccess { get; set; }

            /// <summary>
            /// 获取或设置数据库上下文对象的命令解释适配器.
            /// </summary>
            public ParserAdapter Parser { get; set; }
        }
    }
    /// <summary>
    /// 用于执行配置 <see cref="DatabaseContext"/> 对象的连接池的委托.
    /// </summary>
    /// <param name="read">读访问的数据库引擎对象实例.</param>
    /// <param name="write">写访问的数据库引擎.</param>
    public delegate void ConnectionPoolConfigureHandler(DataEngine read, DataEngine write);
}
