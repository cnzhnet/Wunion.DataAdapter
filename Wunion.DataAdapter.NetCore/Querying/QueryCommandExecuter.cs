using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CodeFirst;

namespace Wunion.DataAdapter.Kernel.Querying
{
    /// <summary>
    /// 查询命令执行器.
    /// </summary>
    public sealed class QueryCommandExecuter
    {
        private DbCommandBuilder commandBuilder;
        private DbContext dbContext;

        /// <summary>
        /// 创建一个查询命令的执行器.
        /// </summary>
        /// <param name="db">数据库上下文.</param>
        /// <param name="cb">数据库查询命令构建器.</param>
        internal QueryCommandExecuter(DbContext db, DbCommandBuilder cb)
        {
            dbContext = db;
            commandBuilder = cb;
        }

        /// <summary>
        /// 执行命令并返回一个数据读取器.
        /// </summary>
        /// <param name="controller">在其中执行命令的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        /// <returns></returns>
        private IDataReader ExecuteReader(DataEngine engine, object controller = null)
        {
            if (controller == null)
            {
                IDataReader reader = engine.ExecuteReader(commandBuilder);
                if (reader == null)
                {
                    string message = (engine.DBA.Error != null) ? engine.DBA.Error.Message : "An unknown error occurred while executing the query.";
                    throw new Exception(message);
                }
                return reader;
            }
            // 在事务中执行查询.
            DBTransactionController trans = controller as DBTransactionController;
            if (trans != null)
                return trans.DBA.ExecuteReader(commandBuilder);
            // 在批处理中执行查询.
            BatchCommander batch = controller as BatchCommander;
            if (batch != null)
                return batch.ExecuteReader(commandBuilder);

            throw new NotSupportedException("Incorrect parameter type: controller");
        }

        /// <summary>
        /// 指行查询并返回结果中第一行第一列的数据.
        /// </summary>
        /// <param name="controller">在其中执行命令的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public object ExecuteScalar(object controller = null)
        {
            object result;
            if (controller == null)
            {
                DataEngine engine = dbContext.DbEngine;
                result = engine.ExecuteScalar(commandBuilder);
                if (engine.DBA.Error != null)
                    throw new Exception(engine.DBA.Error.Message);
                return result;
            }
            // 在事务中执行.
            DBTransactionController trans = controller as DBTransactionController;
            if (trans != null)
            {
                result = trans.DBA.ExecuteScalar(commandBuilder);
                return result;
            }
            // 在批处理中执行.
            BatchCommander batch = controller as BatchCommander;
            if (batch != null)
            {
                result = batch.QueryScalar(commandBuilder);
                return result;
            }
            throw new NotSupportedException("Incorrect parameter type: controller");
        }

        /// <summary>
        ///  执行查询并将结果返回为非实体对象集合.
        /// </summary>
        /// <typeparam name="T">目标对象类型.</typeparam>
        /// <param name="controller">在其中执行命令的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        /// <returns></returns>
        public List<T> ToList<T>(object controller = null) where T : class, new()
        {
            List<T> queryResult;
            DataEngine engine = dbContext.DbEngine;
            using (IDataReader reader = ExecuteReader(engine, controller))
            {
                queryResult = reader.ToList<T>(engine);
            }
            return queryResult;
        }

        /// <summary>
        /// 执行查询并将结果返回为实体集合.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="controller">在其中执行命令的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        /// <returns></returns>
        public List<TEntity> ToEntityList<TEntity>(object controller = null) where TEntity : class, new()
        {
            List<TEntity> queryResult = null;
            DataEngine engine = dbContext.DbEngine;
            using (IDataReader reader = ExecuteReader(engine, controller))
            {
                queryResult = reader.ToEntityList<TEntity>(engine);
            }
            return queryResult;
        }

        /// <summary>
        /// 执行查询并将结果返回为动态实体集合.
        /// </summary>
        /// <param name="controller">在其中执行命令的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        /// <param name="converter">用于转换字段值的数据类型.</param>
        /// <returns></returns>
        public List<dynamic> ToDynamicList(object controller = null, Func<string, object, Type, object> converter = null)
        {
            List<dynamic> queryResult = null;
            DataEngine engine = dbContext.DbEngine;
            using (IDataReader reader = ExecuteReader(engine, controller))
            {
                queryResult = reader.ToDynamicList(converter);
            }
            return queryResult;
        }
    }
}
