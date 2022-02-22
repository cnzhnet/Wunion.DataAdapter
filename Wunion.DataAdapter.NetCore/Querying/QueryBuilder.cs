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
    /// 表示数据库的查询构建器.
    /// </summary>
    /// <typeparam name="TDAO"></typeparam>
    public class QueryBuilder<TDAO> where TDAO : QueryDao, new()
    {
        private TDAO dao;
        private List<IDbTableContext> contexts;
        private Dictionary<Type, QueryDao> queryDaos;
        private IDescription[] queryFields;
        private List<IDescription> froms; 
        private List<object[]> conditions;
        private IncludeQueryBuilder<TDAO> includeQuery;
        private DbCommandBuilder cb;

        /// <summary>
        /// 创建一个查询构建器的对象实例.
        /// </summary>
        /// <param name="table">主要查询的数据表上下文.</param>
        internal QueryBuilder(IDbTableContext table)
        {
            dao = new TDAO { db = table.db, TableName = table.tableName };
            contexts = new List<IDbTableContext>();
            queryDaos = new Dictionary<Type, QueryDao>();
            froms = new List<IDescription>();
            conditions = new List<object[]>();

            contexts.Add(table);
            froms.Add(fm.Table(table.tableName));
            queryDaos.Add(typeof(TDAO), dao);
            includeQuery = new IncludeQueryBuilder<TDAO>(queryDaos);
        }

        private void GenerateTablesName()
        {
            bool yes = contexts.Count > 1;
            foreach (KeyValuePair<Type, QueryDao> itm in queryDaos)
                itm.Value.GenerateTableName = yes;
        }

        /// <summary>
        /// 设置查询构建器的关联查询.
        /// </summary>
        /// <typeparam name="TForeignDao"></typeparam>
        /// <param name="foreignDao"></param>
        /// <param name="t"></param>
        /// <param name="expression"></param>
        private void SetInclude<TForeignDao>(TForeignDao foreignDao, Type t, Func<IncludeQueryBuilder<TDAO>, object[]> expression) where TForeignDao : QueryDao, new()
        {
            queryDaos.Add(t, foreignDao);
            // 外键约束设置正常则继续添加联合表.
            IDbTableContext dbTable = foreignDao.GetTableContext();
            contexts.Add(dbTable);
            GenerateTablesName();
            froms.Add(fm.LeftJoin(
                fm.Table(dbTable.tableName))
                  .ON(expression(includeQuery))
            );
        }

        /// <summary>
        /// 在该查询中包含一个外键关联的联合查询，若要查询的表与指定的泛型参数 TForeignDao 数据访问器相关的表未定义外键关系，则引发 <see cref="NotSupportedException"/>异常.
        /// </summary>
        /// <typeparam name="TForeignDao">外键对应的数据访问器.</typeparam>
        /// <param name="expression">联合的表要查出的定段.</param>
        /// <exception cref="NotSupportedException">查询的表与指定的泛型参数 TForeignDao 数据访问器相关的表未定义外键关系时发生.</exception>
        /// <returns></returns>
        public QueryBuilder<TDAO> Include<TForeignDao>(Func<IncludeQueryBuilder<TDAO>, object[]> expression) where TForeignDao : QueryDao, new()
        {
            Type t = typeof(TForeignDao);
            if (queryDaos.ContainsKey(t))
                throw new Exception($"The query already contains the foreign key table related to {t.Name}.");
            // 先判断要查询的主表的外键设置与 TParentDao 是否匹配.
            TForeignDao foreignDao = new TForeignDao { db = dao.db };
            SetInclude<TForeignDao>(foreignDao, t, expression);
            return this;
        }

        /// <summary>
        /// 在该查询中包含一个外键关联的联合查询，若要查询的表与指定的泛型参数 TForeignDao 数据访问器相关的表未定义外键关系，则引发 <see cref="NotSupportedException"/>异常.
        /// </summary>
        /// <typeparam name="TForeignDao">外键对应的数据访问器.</typeparam>
        /// <param name="tableName">显示指定要关联的表名称.</param>
        /// <param name="expression">联合的表要查出的定段.</param>
        /// <exception cref="NotSupportedException">查询的表与指定的泛型参数 TForeignDao 数据访问器相关的表未定义外键关系时发生.</exception>
        /// <returns></returns>
        public QueryBuilder<TDAO> Include<TForeignDao>(string tableName, Func<IncludeQueryBuilder<TDAO>, object[]> expression) where TForeignDao : QueryDao, new()
        {
            Type t = typeof(TForeignDao);
            if (queryDaos.ContainsKey(t))
                throw new Exception($"The query already contains the foreign key table related to {t.Name}.");
            // 先判断要查询的主表的外键设置与 TParentDao 是否匹配.
            TForeignDao foreignDao = new TForeignDao { 
                db = dao.db ,
                TableName = tableName
            };
            SetInclude<TForeignDao>(foreignDao, t, expression);
            return this;
        }

        /// <summary>
        /// 构建查询条件.
        /// </summary>
        /// <typeparam name="TargetDao">目标实体的查询娄据访问器类型.</typeparam>
        /// <param name="condition">用于返回查询条件的方法.</param>
        /// <returns></returns>
        public QueryBuilder<TDAO> Where<TargetDao>(Func<TargetDao, object[]> condition) where TargetDao : QueryDao
        {
            if (conditions.Count > 0)
                throw new Exception("Now, you must use the method: AndWhere(...) or OrWhere(...)");
            Type t = typeof(TargetDao);
            QueryDao targetDao = null;
            if (!queryDaos.TryGetValue(t, out targetDao))
                throw new Exception($"{t.Name} is not included in the query.");
            conditions.Add(condition((TargetDao)targetDao));
            return this;
        }

        /// <summary>
        /// 构建与前一个条件形成逻辑与（即 and ）运算的查询条件.
        /// </summary>
        /// <typeparam name="TargetDao"></typeparam>
        /// <param name="condition">用于返回查询条件</param>
        /// <returns></returns>
        public QueryBuilder<TDAO> AndWhere<TargetDao>(Func<TargetDao, object[]> condition) where TargetDao : QueryDao
        {
            Type t = typeof(TargetDao);
            QueryDao targetDao = null;
            if (!queryDaos.TryGetValue(t, out targetDao))
                throw new Exception($"{t.Name} is not included in the query.");
            List<object> objects = new List<object>(new object[] { exp.And });
            objects.AddRange(condition((TargetDao)targetDao));
            conditions.Add(objects.ToArray());
            return this;
        }

        /// <summary>
        /// 构建与前一个条件形成逻辑与（即 or ）或算的查询条件.
        /// </summary>
        /// <typeparam name="TargetDao"></typeparam>
        /// <param name="condition">用于返回查询条件</param>
        /// <returns></returns>
        public QueryBuilder<TDAO> OrWhere<TargetDao>(Func<TargetDao, object[]> condition) where TargetDao : QueryDao
        {
            Type t = typeof(TargetDao);
            QueryDao targetDao = null;
            if (!queryDaos.TryGetValue(t, out targetDao))
                throw new Exception($"{t.Name} is not included in the query.");
            List<object> objects = new List<object>(new object[] { exp.Or });
            objects.AddRange(condition((TargetDao)targetDao));
            conditions.Add(objects.ToArray());
            return this;
        }

        /// <summary>
        /// 选择要查询的字段.
        /// </summary>
        /// <param name="selector">用于选择字段.</param>
        /// <returns></returns>
        public QueryBuilder<TDAO> Select(Func<IncludeQueryBuilder<TDAO>, IDescription[]> selector) 
        {
            DbCommandBuilder cb = new DbCommandBuilder();
            queryFields = selector(includeQuery);
            return this;
        }

        /// <summary>
        /// 构建 COUNT 查询命令.
        /// </summary>
        /// <returns></returns>
        private DbCommandBuilder CountBuild()
        {
            DbCommandBuilder cmdb = new DbCommandBuilder();
            SelectBlock sb = cmdb.Select(Fun.Count(1)).From(froms.ToArray());
            List<object> tmp = new List<object>();
            foreach (object[] array in conditions)
                tmp.AddRange(array);
            if (tmp.Count > 0)
                sb.Where(tmp.ToArray());
            return cmdb;
        }

        /// <summary>
        /// 构造查询命令.
        /// </summary>
        /// <param name="options">用于设置更多的查询选项，例如排序、分页等.</param>
        /// <returns></returns>
        public QueryBuilder<TDAO> Build(Action<IncludeQueryBuilder<TDAO>, SelectBlock> options = null)
        {
            if (queryFields == null || queryFields.Length < 1)
                throw new NullReferenceException("You must be called the method \"Select(...)\" before build the command.");
            cb = new DbCommandBuilder();
            SelectBlock sb = cb.Select(queryFields).From(froms.ToArray());
            List<object> tmp = new List<object>();
            foreach (object[] array in conditions)
                tmp.AddRange(array);
            if (tmp.Count > 0)
                sb.Where(tmp.ToArray());
            options?.Invoke(includeQuery, sb);
            return this;
        }

        /// <summary>
        /// 执行命令并返回一个数据读取器.
        /// </summary>
        /// <param name="controller">在其中执行命令的事务控制器(<see cref="DBTransactionController"/>)或批处理(<see cref="BatchCommander"/>)对象.</param>
        /// <returns></returns>
        private IDataReader ExecuteReader(DataEngine engine, object controller = null)
        {
            if (cb == null)
                throw new Exception("The query command has not been build.");
            if (controller == null)
            {
                IDataReader reader = engine.ExecuteReader(cb);
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
                return trans.DBA.ExecuteReader(cb);
            // 在批处理中执行查询.
            BatchCommander batch = controller as BatchCommander;
            if (batch != null)
                return batch.ExecuteReader(cb);

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
            DataEngine engine = contexts.First().db.DbEngine;
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
            DataEngine engine = contexts.First().db.DbEngine;
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
            DataEngine engine = contexts.First().db.DbEngine;
            using (IDataReader reader = ExecuteReader(engine, controller))
            {
                queryResult = reader.ToDynamicList(converter);
            }
            return queryResult;
        }

        /// <summary>
        /// 查询符合的记录数量.
        /// </summary>
        /// <param name="controller">在该批处理器中执行查询.</param>
        /// <returns></returns>
        public int Count(object controller = null)
        {
            DbCommandBuilder ccount = CountBuild();
            object result;
            if (controller == null)
            {
                DataEngine engine = contexts.First().db.DbEngine;
                result = engine.ExecuteScalar(ccount);
                if (engine.DBA.Error != null)
                    throw new Exception(engine.DBA.Error.Message);
                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
            // 在事务中执行.
            DBTransactionController trans = controller as DBTransactionController;
            if (trans != null)
            {
                result = trans.DBA.ExecuteScalar(ccount);
                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
            // 在批处理中执行.
            BatchCommander batch = controller as BatchCommander;
            if (batch != null)
            {
                result = batch.QueryScalar(ccount);
                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
            throw new NotSupportedException("Incorrect parameter type: controller");
        }

        /// <summary>
        /// 创建一个查询构建器.
        /// </summary>
        /// <typeparam name="TDao">查询数据访问器的类型.</typeparam>
        /// <param name="table">默认要查询的数据表.</param>
        /// <returns></returns>
        public static QueryBuilder<TDAO> Create(IDbTableContext table)
        {
            return new QueryBuilder<TDAO>(table);
        }
    }
}
