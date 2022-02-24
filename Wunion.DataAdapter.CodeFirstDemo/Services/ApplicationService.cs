using System;
using System.Collections.Generic;
using System.Linq;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.Querying;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel;

namespace Wunion.DataAdapter.CodeFirstDemo.Services
{
    /// <summary>
    /// 应用程序服务的基础实现类型.
    /// </summary>
    /// <typeparam name="TData">数据实体或模型的类型名称.</typeparam>
    /// <typeparam name="TDao">实体的数据查询访问类型名称.</typeparam>
    public abstract class ApplicationService<TData> where TData : class, new()
    {
        /// <summary>
        /// 创建一个 <see cref="ApplicationService{TData}"/> 的对象实例.
        /// </summary>
        /// <param name="context">数据库上下文对象.</param>
        protected ApplicationService(MyDbContext context)
        {
            db = context;
        }

        /// <summary>
        /// 数据库上下文对象.
        /// </summary>
        protected MyDbContext db { get; private set; }

        /// <summary>
        /// 添加数据.
        /// </summary>
        /// <param name="data">要添加的数据.</param>
        public abstract void Add(TData data);

        /// <summary>
        /// 更新数据.
        /// </summary>
        /// <param name="data">要更新的数据.</param>
        public abstract void Update(TData data);

        /// <summary>
        /// 删除数据.
        /// </summary>
        /// <param name="condition">删除条件.</param>
        public abstract void Delete(object condition);

        /// <summary>
        /// 支持软删除的实现.
        /// </summary>
        /// <typeparam name="TEntity">实体类型名称.</typeparam>
        /// <typeparam name="TDao">查询访问器类型名称.</typeparam>
        /// <param name="tableContext">表上下文对象.</param>
        /// <param name="condition">删除条件</param>
        /// <param name="controller">执行删除的的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        protected void Delete<TEntity, TDao>(DbTableContext<TEntity> tableContext, Func<TDao, object[]> condition, object controller = null) 
            where TEntity : class, new() where TDao : QueryDao, new()
        {
            TEntity entity = new TEntity();
            ISoftDelete softDelete = entity as ISoftDelete;
            if (softDelete == null) // 实体不支持软删除.
            {
                tableContext.Delete(condition, controller);
                return;
            }
            // 实体支持软删除
            TDao dao = new TDao { db = db };
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.Update(tableContext.tableName)
                .Set(
                    dao.GetField(nameof(softDelete.IsDeleted)) == true, 
                    dao.GetField(nameof(softDelete.DeletionDate)) == DateTime.Now
                )
                .Where(condition(dao));
            if (controller == null)
            {
                db.DbEngine.ExecuteNoneQuery(cb);
                return;
            }
            DBTransactionController trans = controller as DBTransactionController;
            if (trans != null)
            {
                trans.DBA.ExecuteNoneQuery(cb);
                return;
            }
            BatchCommander batch = controller as BatchCommander;
            batch?.ExecuteNonQuery(cb);
        }
    }
}
