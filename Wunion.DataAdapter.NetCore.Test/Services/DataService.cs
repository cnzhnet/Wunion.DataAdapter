using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Data;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace Wunion.DataAdapter.NetCore.Test.Services
{
    /// <summary>
    /// 表示数据交互服务的基础对象类型.
    /// </summary>
    public abstract class DataService
    {
        /// <summary>
        /// 创建一个 <see cref="DataService"/> 的对象实例.
        /// </summary>
        protected DataService()
        { }

        /// <summary>
        /// 获取或设置数据库引擎.
        /// </summary>
        public DataEngine db { get; internal set; }

        /// <summary>
        /// 检测并触发数据库异常.
        /// </summary>
        /// <param name="result">数据库引擎返回的执行结果.</param>
        /// <param name="trans">引发错误的事务，若未使用事务时应为 null .</param>
        protected void ThrowDbException(int result, DBTransactionController trans)
        {
            if (result != -1)
                return;
            if (trans == null)
                throw new Exception(db.DBA.Error.Message);
            else
                throw new Exception(trans.DBA.Errors.First().Message);
        }

        /// <summary>
        /// 添加数据，返回自增长编号字段的值.
        /// </summary>
        /// <param name="data">包含添加的数据的对象.</param>
        /// <param name="tableName">表名称.</param>
        /// <param name="trans">在该事务中执行添加任务，若无事务则为 null.</param>
        /// <returns></returns>
        public virtual object Add(Dictionary<string, object> data, string tableName, DBTransactionController trans = null)
        {
            QuickDataChanger dc = new QuickDataChanger(trans, db);
            ThrowDbException(dc.SaveToDataBase(tableName, data, false), trans);
            if (trans != null)
                return trans.DBA.SCOPE_IDENTITY;
            return db.DBA.SCOPE_IDENTITY;
        }

        /// <summary>
        /// 更新数据，并返回受影响的记录数.
        /// </summary>
        /// <param name="data">包含要更新的数据的字典.</param>
        /// <param name="tableName">表名称.</param>
        /// <param name="condition">更新条件.</param>
        /// <param name="trans">在该事务中执行更新，若无事务则为 null.</param>
        /// <returns></returns>
        public virtual int Update(Dictionary<string, object> data, string tableName, object[] condition, DBTransactionController trans = null)
        {
            QuickDataChanger dc = new QuickDataChanger(trans, db);
            dc.Conditions.AddRange(condition);
            int result = dc.SaveToDataBase(tableName, data, true);
            ThrowDbException(result, trans);
            return result;
        }

        /// <summary>
        /// 从指定的表中删除指定条件的数据，并返回受影响的记录数.
        /// </summary>
        /// <param name="tableName">表名称.</param>
        /// <param name="condition">删除条件.</param>
        /// <param name="trans">在该事务中执行更新，若无事务则为 null.</param>
        /// <returns></returns>
        public virtual int Delete(string tableName, object[] condition, DBTransactionController trans = null)
        {
            QuickDataChanger dc = new QuickDataChanger(trans, db);
            dc.Conditions.AddRange(condition);
            int result = dc.Delete(tableName);
            ThrowDbException(result, trans);
            return result;
        }

        #region 静态成员
        private static ConcurrentDictionary<Type, DataService> pool = new ConcurrentDictionary<Type, DataService>();

        /// <summary>
        /// 获取指定的数据交互服务.
        /// </summary>
        /// <typeparam name="TService">数据服务的类型名称.</typeparam>
        /// <param name="db">目标数据库引擎对象.</param>
        /// <returns></returns>
        public static TService Get<TService>(DataEngine db) where TService : DataService, new()
        {
            Type t = typeof(TService);
            DataService service = null;
            if (!(pool.TryGetValue(t, out service)))
            {
                service = new TService();
                pool.TryAdd(t, service);
            }
            service.db = db;
            return service as TService;
        }
        #endregion
    }
}
