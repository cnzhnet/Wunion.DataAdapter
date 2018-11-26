using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 表示数据表上下文对象的标准映射器.
    /// </summary>
    public abstract class TableMapper
    {
        /// <summary>
        /// 获取数据表的映射信息.
        /// </summary>
        /// <returns></returns>
        public abstract EntityTableAttribute GetTableAttribute();

        /// <summary>
        /// 获取或设置表名称.
        /// </summary>
        /// <returns></returns>
        internal abstract string GetTableName();

        /// <summary>
        /// 设置用于读及写该表数据的数据引擎.
        /// </summary>
        /// <param name="readEngine">用于读取该表数据的引擎.</param>
        /// <param name="writeEngine">用于写入表数据的引擎.</param>
        internal abstract void SetDataEngine(DataEngine readEngine, DataEngine writeEngine);

        /// <summary>
        /// 创建实体与数据表的映射代理对象实例.
        /// </summary>
        /// <returns></returns>
        public abstract EntityAgent CreateAgent();
    }

    /// <summary>
    /// 数据表的上下文对象.
    /// </summary>
    /// <typeparam name="TEntityAgent">表的实体查询代理类型.</typeparam>
    public abstract class TableContext<TEntityAgent> : TableMapper where TEntityAgent : EntityAgent, new()
    {
        /// <summary>
        /// 用于缓存表映射信息的字典.
        /// </summary>
        private static Dictionary<Type, EntityTableAttribute> TableAttributeCache = new Dictionary<Type, EntityTableAttribute>();

        private DataEngine _Reader, _Writer;
        private string tableName;

        /// <summary>
        /// 创建一个 <see cref="TableContext{TAgent}"/> 的对象实例.
        /// </summary>
        protected TableContext()
        { }

        /// <summary>
        /// 获取读该表数据的引擎.
        /// </summary>
        protected DataEngine Reader => _Reader;

        /// <summary>
        /// 获取或该表数据的引擎.
        /// </summary>
        protected DataEngine Writer => _Writer;

        /// <summary>
        /// 获取数据表的映射信息.
        /// </summary>
        /// <returns></returns>
        public override EntityTableAttribute GetTableAttribute()
        {
            Type agentType = typeof(TEntityAgent);
            if (TableAttributeCache.ContainsKey(agentType))
                return TableAttributeCache[agentType];

            object[] Attributes = agentType.GetCustomAttributes(typeof(EntityTableAttribute), true);
            if (Attributes == null || Attributes.Length < 0)
                throw new NotSupportedException(string.Format("Not supported table context object, please use {1} for class: {0}.", typeof(EntityTableAttribute).FullName, agentType.FullName));
            EntityTableAttribute tableAttribute = (EntityTableAttribute)Attributes[0];
            TableAttributeCache.Add(agentType, tableAttribute);
            return tableAttribute;
        }

        /// <summary>
        /// 获取或设置表名称.
        /// </summary>
        /// <returns></returns>
        internal override string GetTableName()
        {
            if (string.IsNullOrEmpty(tableName))
            {
                EntityTableAttribute tableAttribute = GetTableAttribute();
                StringBuilder tabName = new StringBuilder();
                tabName.Append(Reader.CommandParserAdapter.ElemIdentifierL);
                tabName.Append(tableAttribute.TableName);
                tabName.Append(Reader.CommandParserAdapter.ElemIdentifierR);
                tableName = tabName.ToString();
                return tabName.ToString();
            }
            else
            {
                return tableName;
            }
        }

        /// <summary>
        /// 设置用于读及写该表数据的数据引擎.
        /// </summary>
        /// <param name="readEngine">用于读取该表数据的引擎.</param>
        /// <param name="writeEngine">用于写入表数据的引擎.</param>
        internal override void SetDataEngine(DataEngine readEngine, DataEngine writeEngine)
        {
            _Reader = readEngine;
            _Writer = writeEngine;
        }

        /// <summary>
        /// 创建实体与数据表的映射代理对象实例.
        /// </summary>
        /// <returns></returns>
        public override EntityAgent CreateAgent()
        {
            TEntityAgent agent = new TEntityAgent();
            agent.TableContext = this;
            return agent;
        }

        /// <summary>
        /// 执行数据库查询.
        /// </summary>
        /// <typeparam name="TEntity">返回的集合实体类型.</typeparam>
        /// <param name="Command">用于执行数据库查询的命令构建器对象.</param>
        /// <returns></returns>
        protected List<TEntity> ExecuteQuery<TEntity>(DbCommandBuilder Command) where TEntity : DataEntity, new()
        {
            IDataReader Rd = Reader.ExecuteReader(Command);
            if (Rd == null)
            {
                string ErrorMessage = (Reader.DBA.Error == null) ? "查询数据时产生了未知的错误．" : Reader.DBA.Error.Message;
                throw new Exception(ErrorMessage);
            }
            List<TEntity> Result = new List<TEntity>();
            TEntity entity;
            string Field;
            int i;
            while (Rd.Read())
            {
                entity = new TEntity();
                for (i = 0; i < Rd.FieldCount; ++i)
                {
                    Field = Rd.GetName(i);
                    entity.SetValue(Field, Rd.GetValue(i), true);
                }
                Result.Add(entity);
            }
            Rd.Close();
            Rd.Dispose();
            return Result;
        }

        /// <summary>
        /// 查询指定实体对应的表，并返回数据集合.
        /// </summary>
        /// <typeparam name="TEntity">对应数据库表的实体类型.</typeparam>
        /// <param name="selector">返回查询条件的方法.</param>
        /// <exception cref="NotSupportedException">当<c>TEntity</c>指定的实体类型未标记<see cref="EntityTableAttribute"/>属性时引发该异常</exception>
        /// <exception cref="Exception">当数据库查询过程产生错误时引发该异常.</exception>
        /// <returns></returns>
        protected List<TEntity> Select<TEntity>(Func<TEntityAgent, object> selector) where TEntity : DataEntity, new()
        {
            EntityTableAttribute entityTable = GetTableAttribute();
            DbCommandBuilder Command = new DbCommandBuilder();
            TEntityAgent agent = new TEntityAgent();
            agent.TableContext = this;
            object Conditions = selector(agent);
            if (Conditions == null)
                Command.Select(td.Field("*")).From(entityTable.TableName);
            else
                Command.Select(td.Field("*")).From(entityTable.TableName).Where(Conditions);
            return ExecuteQuery<TEntity>(Command);
        }

        /// <summary>
        /// 根据指定的条件查询指定实体对应的表，并返回数据集合.
        /// </summary>
        /// <typeparam name="TEntity">对应数据库表的实体类型.</typeparam>
        /// <param name="action">用于指定查询条件.</param>
        /// <returns></returns>
        protected List<TEntity> Select<TEntity>(Action<TEntityAgent, SelectBlock> action) where TEntity : DataEntity, new()
        {
            EntityTableAttribute entityTable = GetTableAttribute();
            DbCommandBuilder Command = new DbCommandBuilder();
            SelectBlock select = Command.Select(td.Field("*")).From(entityTable.TableName);
            TEntityAgent agent = new TEntityAgent();
            agent.TableContext = this;
            action(agent, select);
            return ExecuteQuery<TEntity>(Command);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="action">用于构建查询命令.</param>
        /// <returns></returns>
        public object QueryScalar(Action<TEntityAgent, TableQuerySelector> action)
        {
            EntityTableAttribute entityTable = GetTableAttribute();
            TEntityAgent agent = new TEntityAgent();
            agent.TableContext = this;
            TableQuerySelector selector = new TableQuerySelector(entityTable.TableName);
            action(agent, selector);
            object Result = Reader.ExecuteScalar(selector.DbCommand);
            if (Result == null && Reader.DBA.Error != null)
                throw new Exception(Reader.DBA.Error.Message);
            return Result;
        }

        /// <summary>
        /// 过滤实体的自增长字段，使之不被更新或插入.
        /// </summary>
        /// <param name="entityType">实体对象的类型.</param>
        /// <returns></returns>
        protected string[] GetWithOutFields(Type entityType)
        {
            List<string> withOutFields = new List<string>();
            PropertyInfo[] ps = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            EntityPropertyAttribute entityProperty;
            foreach (PropertyInfo p in ps)
            {
                entityProperty = p.GetCustomAttribute(typeof(EntityPropertyAttribute)) as EntityPropertyAttribute;
                if (entityProperty == null)
                    continue;
                if (entityProperty.IsIdentity)
                    withOutFields.Add(p.Name);
            }
            return withOutFields.ToArray();
        }

        /// <summary>
        /// 向数据表中添加一条记录.
        /// </summary>
        /// <typeparam name="TEntity">数据表对应的实体类型.</typeparam>
        /// <param name="entity">实体对象.</param>
        /// <param name="trans">写事务的事务控制器.</param>
        /// <exception cref="Exception">当新增数据失败时引发该异常.</exception>
        protected void Add<TEntity>(TEntity entity, DBTransactionController trans = null) where TEntity : DataEntity, new()
        {
            if (entity == null)
                return;
            // 增加记录时过滤掉自增长字段.
            string[] withOutFields = GetWithOutFields(entity.GetType());
            // 向数据库中增加记录
            bool InTransaction = trans != null;
            EntityTableAttribute tableAttribute = GetTableAttribute();
            QuickDataChanger DC;
            if (InTransaction)
                DC = new QuickDataChanger(trans, Writer);
            else
                DC = new QuickDataChanger(Writer);
            int result = DC.SaveToDataBase(tableAttribute.TableName, entity.ToDictionary(withOutFields), false);
            if (result == -1) // 若添加失败则向外界抛出异常.
            {
                string Error;
                if (InTransaction)
                    Error = (trans.DBA.Errors != null && trans.DBA.Errors.Count > 0) ? trans.DBA.Errors[0].Message : string.Format("新增：{0}是产生未知错误.", entity.GetType().Name);
                else
                    Error = (Writer.DBA.Error == null) ? string.Format("新增：{0}是产生未知错误.", entity.GetType().Name) : Writer.DBA.Error.Message;
                throw new Exception(Error);
            }
            else // 新增成功时更新实体中的自增长字段值.
            {
                if (withOutFields.Length > 0)
                {
                    if (InTransaction)
                        entity.SetValue(withOutFields[withOutFields.Length - 1], trans.DBA.SCOPE_IDENTITY, true);
                    else
                        entity.SetValue(withOutFields[withOutFields.Length - 1], Writer.DBA.SCOPE_IDENTITY, true);
                }
            }
        }

        /// <summary>
        /// 更新数据表中的记录.
        /// </summary>
        /// <typeparam name="TEntity">数据表对应的实体类型.</typeparam>
        /// <param name="entity">实体对象.</param>
        /// <param name="func">用于创建更新条件.</param>
        /// <exception cref="ArgumentNullException">当必备的参数为空时引发此异常.</exception>
        /// <exception cref="Exception">当执行更新时产生错误时引发此异常.</exception>
        /// <returns>返回受影响的记录数.</returns>
        protected int Update<TEntity>(TEntity entity, Func<TEntityAgent, object[]> func) where TEntity : DataEntity, new()
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (func == null)
                throw new Exception("未指定更新条件时，整个数据表中的所有数据都将被更新为一样的，此操作已被禁止.");
            // 过滤掉自增长字段.
            string[] withOutFields = GetWithOutFields(entity.GetType());
            EntityTableAttribute tableAttribute = GetTableAttribute();
            QuickDataChanger DC = new QuickDataChanger(Writer);
            if (func != null)
            {
                TEntityAgent agent = new TEntityAgent();
                agent.TableContext = this;
                DC.Conditions.AddRange(func(agent));
            }
            int result = DC.SaveToDataBase(tableAttribute.TableName, entity.ToDictionary(withOutFields), true);
            if (result == -1) // 若更新失败时向外界抛出异常.
                throw new Exception(Writer.DBA.Error.Message);
            return result;
        }

        /// <summary>
        /// 在事务中更新数据表中的记录.
        /// </summary>
        /// <typeparam name="TEntity">数据表对应的实体类型.</typeparam>
        /// <param name="trans">写事务的事务控制器.</param>
        /// <param name="entity">实体对象.</param>
        /// <param name="func">用于创建更新条件.</param>
        /// <exception cref="ArgumentNullException">当必备的参数为空时引发此异常.</exception>
        /// <exception cref="Exception">当执行更新时产生错误时引发此异常.</exception>
        /// <returns>返回受影响的记录数.</returns>
        protected int Update<TEntity>(DBTransactionController trans, TEntity entity, Func<TEntityAgent, object[]> func) where TEntity : DataEntity, new()
        {
            if (trans == null)
                throw new ArgumentNullException(nameof(trans));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (func == null)
                throw new Exception("未指定更新条件时，整个数据表中的所有数据都将被更新为一样的，此操作已被禁止.");
            // 过滤掉自增长字段.
            // 过滤掉自增长字段.
            string[] withOutFields = GetWithOutFields(entity.GetType());
            EntityTableAttribute tableAttribute = GetTableAttribute();
            QuickDataChanger DC = new QuickDataChanger(trans, Writer);
            if (func != null)
            {
                TEntityAgent agent = new TEntityAgent();
                agent.TableContext = this;
                DC.Conditions.AddRange(func(agent));
            }
            trans.DBA.Errors.Clear();
            int result = DC.SaveToDataBase(tableAttribute.TableName, entity.ToDictionary(withOutFields), true);
            if (result == -1) // 若更新失败时向外界抛出异常.
            {
                string Error;
                if (trans.DBA.Errors.Count < 1)
                    Error = "更新表时产生了未知的异常.";
                else
                    Error = trans.DBA.Errors[0].Message;
                throw new Exception(Error);
            }
            return result;
        }

        /// <summary>
        /// 删除数据表中满足指定条件的数据.
        /// </summary>
        /// <param name="func">用于创建删除条件.</param>
        /// <exception cref="Exception">当未指定更新条件或删除过程产生错误时引发此异常.</exception>
        /// <returns>返回受影响记录数.</returns>
        public int Delete(Func<TEntityAgent, object[]> func)
        {
            if (func == null)
                throw new Exception("在不指定条件的情况下将会删除整个数据表中的所有数据，该操作已被禁止.");
            EntityTableAttribute tableAttribute = GetTableAttribute();
            QuickDataChanger DC = new QuickDataChanger(Writer);
            TEntityAgent agent = new TEntityAgent();
            agent.TableContext = this;
            DC.Conditions.AddRange(func(agent));
            int result = DC.Delete(tableAttribute.TableName);
            if (result == -1)
                throw new Exception(Writer.DBA.Error.Message);
            return result;
        }

        /// <summary>
        /// 在事务中删除数据表中满足指定条件的数据.
        /// </summary>
        /// <param name="trans">写事务的事务控制器.</param>
        /// <param name="func">用于创建删除条件.</param>
        /// <exception cref="ArgumentNullException">当未指派事务控制器时引发此异常.</exception>
        /// <exception cref="Exception">当未指定更新条件或删除过程产生错误时引发此异常.</exception>
        /// <returns></returns>
        public int Delete(DBTransactionController trans, Func<TEntityAgent, object[]> func)
        {
            if (trans == null)
                throw new ArgumentNullException(nameof(trans));
            if (func == null)
                throw new Exception("在不指定条件的情况下将会删除整个数据表中的所有数据，该操作已被禁止.");
            EntityTableAttribute tableAttribute = GetTableAttribute();
            QuickDataChanger DC = new QuickDataChanger(trans, Writer);
            TEntityAgent agent = new TEntityAgent();
            agent.TableContext = this;
            DC.Conditions.AddRange(func(agent));
            trans.DBA.Errors.Clear();
            int result = DC.Delete(tableAttribute.TableName);
            if (result == -1)
            {
                if (trans.DBA.Errors != null && trans.DBA.Errors.Count > 0)
                    throw new Exception(trans.DBA.Errors[0].Message);
                else
                    throw new Exception("删除数据是产生了未知的错误.");
            }
            return result;
        }
    }
}
