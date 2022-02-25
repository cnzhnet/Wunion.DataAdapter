using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.Querying;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于实现数据库表上下文的接口.
    /// </summary>
    public interface IDbTableContext
    {
        /// <summary>
        /// 数据库引擎对象.
        /// </summary>
        DbContext db { get; }

        /// <summary>
        /// 获取表名称.
        /// </summary>
        string tableName { get; }

        /// <summary>
        /// 该表的所有查询字段信息（避免在查询中使用 select * from ...）
        /// </summary>
        IDescription[] QueryFields { get; }
    }

    /// <summary>
    /// 表示数据库中的表.
    /// </summary>
    /// <typeparam name="TEntity">该表对应的实体类型.</typeparam>
    public class DbTableContext<TEntity> : IDbTableContext where TEntity : class, new()
    {
        /// <summary>
        /// 数据库引擎对象.
        /// </summary>
        public DbContext db { get; private set; }

        /// <summary>
        /// 表示该表在数据库中的表名.
        /// </summary>
        public string tableName { get; private set; }

        /// <summary>
        /// 该表的所有查询字段信息（避免在查询中使用 select * from ...）
        /// </summary>
        public IDescription[] QueryFields => GetQueryFields();

        /// <summary>
        /// 获取该表对应的实体类型.
        /// </summary>
        internal Type EntityType => typeof(TEntity);

        /// <summary>
        /// 创建一个 <see cref="DbTableContext{TEntity}" /> 数据库表的对象实例.
        /// </summary>
        /// <param name="dbEngine">数据库对象.</param>
        /// <param name="name">该表在数据库中的表名.</param>
        public DbTableContext(DbContext dbContext, string name)
        {
            db = dbContext;
            tableName = name;
        }

        /// <summary>
        /// 根据查询的执行结果，若产生错误则抛出异常.
        /// </summary>
        /// <param name="code">查询执行的结果状态.</param>
        /// <param name="err">错误信息.</param>
        protected void ThrowExceptionExecResult(int code, DbError err)
        {
            if (code != -1 || err == null) //查询执行正常
                return;
            string message = err == null ? "An unknown error occurred while executing the query." : err.Message;
            throw new Exception(message);
        }

        /// <summary>
        /// 将值转换为数据库中对应的类型.
        /// </summary>
        /// <param name="mp">字段与实体属性的映射对象.</param>
        /// <param name="value">要转换的值.</param>
        /// <returns></returns>
        protected object ConvertToDbValue(DbConversionMapping mp, object value)
        {
            if (value == null)
                return DBNull.Value;
            IDbValueConverter converter = null;
            if (mp.Attribute.ValueConverter == null)
                converter = db.DbEngine.GetValueConverter(mp.Property.PropertyType);
            else
                converter = System.Activator.CreateInstance(mp.Attribute.ValueConverter) as IDbValueConverter;
            if (converter == null)
                return value;
            return converter.ConvertTo(value, mp.Property.PropertyType);
        }

        /// <summary>
        /// 执行指定的命令并返回受影响的记录数.
        /// </summary>
        /// <param name="cb">已构建的命令.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        protected int ExecuteCommand(DbCommandBuilder cb, object controller = null)
        {
            int result;
            if (controller == null)
            {
                result = db.DbEngine.ExecuteNoneQuery(cb);
                ThrowExceptionExecResult(result, db.DbEngine.DBA.Error);
                return result;
            }

            // 在事务中执行.
            DBTransactionController trans = controller as DBTransactionController;
            if (trans != null)
            {
                result = trans.DBA.ExecuteNoneQuery(cb);
                ThrowExceptionExecResult(result, trans.DBA.Errors.LastOrDefault());
                return result;
            }

            // 在批处理中执行.
            BatchCommander batch = controller as BatchCommander;
            if (batch != null)
            {
                result = batch.ExecuteNonQuery(cb);
                return result;
            }
            throw new NotSupportedException("Incorrect parameter type: controller");
        }

        /// <summary>
        /// 获取实体的字段映射.
        /// </summary>
        /// <returns></returns>
        protected List<DbConversionMapping> GetDbConversionMappings()
        {
            Type t = typeof(TEntity);
            List<DbConversionMapping> mappings = null;
            if (!db.DbEngine.DbConversionPool.TryGetValue(t, out mappings))
            {
                mappings = DbConversionMapping.Get(t);
                if (mappings.Count > 0)
                    db.DbEngine.DbConversionPool.TryAdd(t, mappings);
            }
            return mappings;
        }

        /// <summary>
        /// 获取指定实体的所有查询字段信息（避免在查询中使用 select * from ...）
        /// </summary>
        /// <typeparam name="T">实体的类型名称.</typeparam>
        /// <returns></returns>
        private IDescription[] GetQueryFields()
        {
            List<IDescription> fields = new List<IDescription>();
            List<DbConversionMapping> mappings = GetDbConversionMappings();
            foreach (DbConversionMapping mp in mappings)
            {
                if (string.IsNullOrEmpty(mp.Attribute.Name))
                    mp.Attribute.Name = mp.Property.Name;
                fields.Add(td.Field(mp.Attribute.Name));
            }
            return fields.ToArray();
        }

        /// <summary>
        /// 将指定的实体插入到指定的表中.
        /// </summary>
        /// <param name="entity">实体对象.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        public virtual int Add(TEntity entity, object controller = null)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("Unable to determine the name of the table in the database.");
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            List<DbConversionMapping> mappings = GetDbConversionMappings();
            if (mappings == null || mappings.Count < 1)
                throw new NotSupportedException(string.Format("The given object does not support this operation, you need to mark the attribute with TableFieldAttribute in the type of that object."));
            List<FieldDescription> fields = new List<FieldDescription>();
            List<object> values = new List<object>();
            object tmpValue;
            foreach (DbConversionMapping mp in mappings)
            {
                tmpValue = mp.Property.GetValue(entity);
                if (string.IsNullOrEmpty(mp.Attribute.Name))
                    mp.Attribute.Name = mp.Property.Name;
                if (mp.Identity != null)
                    continue;            // 不插入自动编号字段.
                fields.Add(td.Field(mp.Attribute.Name));
                values.Add(ConvertToDbValue(mp, tmpValue));
            }
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.Insert(tableName, fields.ToArray()).Values(values.ToArray());
            return ExecuteCommand(cb, controller);
        }

        /// <summary>
        /// 将指定的实体插入到指定的表中（异步方法）.
        /// </summary>
        /// <param name="entity">实体对象.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        public async Task<int> AddAsync(TEntity entity, object controller = null)
        {
            int result = await Task.Run<int>(() => Add(entity, controller));
            return result;
        }

        /// <summary>
        /// 将指定的实体更新到数据库中.
        /// </summary>
        /// <param name="entity">实体对象.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        public virtual void Update(TEntity entity, object controller = null)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("Unable to determine the name of the table in the database.");
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            List<DbConversionMapping> mappings = GetDbConversionMappings();
            if (mappings == null || mappings.Count < 1)
                throw new NotSupportedException(string.Format("The given object does not support this operation, you need to mark the attribute with TableFieldAttribute in the type of that object."));

            List<IDescription> expressions = new List<IDescription>();
            List<object> conditions = new List<object>();
            object tmpValue;
            foreach (DbConversionMapping mp in mappings)
            {
                tmpValue = ConvertToDbValue(mp, mp.Property.GetValue(entity));
                if (string.IsNullOrEmpty(mp.Attribute.Name))
                    mp.Attribute.Name = mp.Property.Name;
                if (mp.Attribute.PrimaryKey) //将主键作为更新条件.
                {
                    if (conditions.Count > 0)
                        conditions.Add(exp.And);
                    conditions.Add(tmpValue == DBNull.Value ? td.Field(mp.Attribute.Name).IsNull() : td.Field(mp.Attribute.Name) == tmpValue);
                }
                else // 非主键则进行更新
                {
                    if (mp.Identity == null) // 仅更新非自动编号字段.
                        expressions.Add(td.Field(mp.Attribute.Name) == tmpValue);
                }
            }
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.Update(tableName).Set(expressions.ToArray()).Where(conditions.ToArray());
            ExecuteCommand(cb, controller);
        }

        /// <summary>
        /// 将指定的实体更新到数据库中（异步方法）.
        /// </summary>
        /// <param name="entity">实体对象.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        public async Task UpdateAsync(TEntity entity, object controller = null)
        { 
            Task t = Task.Run(() => Update(entity, controller));
            await t;
        }

        /// <summary>
        /// 从数据库中删除指定条件的数据.
        /// </summary>
        /// <param name="conditions">用于确定删除条件.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        public void Delete<TQueryDAO>(Func<TQueryDAO, object[]> conditions, object controller = null) where TQueryDAO : QueryDao, new()
        {
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("Unable to determine the name of the table in the database.");
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.Delete(tableName).Where(conditions(new TQueryDAO { db = db }));
            ExecuteCommand(cb, controller);
        }

        /// <summary>
        /// 从数据库中删除指定条件的数据.
        /// </summary>
        /// <param name="conditions">用于确定删除条件.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        public async Task DeleteAsync<TQueryDAO>(Func<TQueryDAO, object[]> conditions, object controller = null) where TQueryDAO : QueryDao, new()
        {
            Task t = Task.Run(() => Delete<TQueryDAO>(conditions, controller));
            await t;
        }

        /// <summary>
        /// 查询符合条件的记录数量.
        /// </summary>
        /// <param name="conditions">条件.</param>
        /// <param name="controller">执行数据插入的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        /// <returns></returns>
        public int Count<TQueryDAO>(Func<TQueryDAO, object[]> conditions = null, object controller = null) where TQueryDAO : QueryDao, new()
        {
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("Unable to determine the name of the table in the database.");
            DbCommandBuilder cb = new DbCommandBuilder();
            if (conditions == null)
            {
                cb.Select(Fun.Count(1)).From(tableName);
            }
            else
            {
                cb.Select(Fun.Count(1)).From(tableName).Where(conditions(new TQueryDAO { db = db }));
            }
            object val;
            // 直接执行.
            if (controller == null)
            {
                val = db.DbEngine.ExecuteScalar(cb);
                if (db.DbEngine.DBA.Error != null)
                    throw new Exception(db.DbEngine.DBA.Error.Message);
                return Convert.ToInt32(val);
            }
            // 在事务中执行.
            DBTransactionController trans = controller as DBTransactionController;
            if (trans != null)
            {
                val = trans.DBA.ExecuteScalar(cb);
                DbError error = trans.DBA.Errors.LastOrDefault();
                if (error != null)
                    throw new Exception(error.Message);
                return Convert.ToInt32(val);
            }
            //在批处理中执行.
            BatchCommander batch = controller as BatchCommander;
            if (batch != null)
            {
                val = batch.QueryScalar(cb);
                return Convert.ToInt32(val);
            }
            throw new NotSupportedException("Incorrect parameter type: controller");
        }

        /// <summary>
        /// 查询符合条件的记录数量（异步方法）.
        /// </summary>
        /// <param name="conditions">条件.</param>
        /// <param name="batch">在此批处理中执行查询.</param>
        /// <returns></returns>
        public async Task<int> CountAsync<TQueryDAO>(Func<TQueryDAO, object[]> conditions = null, BatchCommander batch = null) where TQueryDAO : QueryDao, new()
        {
            int result = await Task.Run<int>(() => Count<TQueryDAO>(conditions, batch));
            return result;
        }
    }
}
