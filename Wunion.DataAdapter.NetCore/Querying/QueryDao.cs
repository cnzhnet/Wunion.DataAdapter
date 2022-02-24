using System;
using System.Collections.Generic;
using System.Linq;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CodeFirst;

namespace Wunion.DataAdapter.Kernel.Querying
{
    /// <summary>
    /// 构建查询的数据访问描述（主用于描述实体属性对应的查询字段命令）.
    /// </summary>
    public abstract class QueryDao
    {
        /// <summary>
        /// 创建一个 <see cref="QueryDao"/> 的对象实例.
        /// </summary>
        /// <param name="dbc">数据库上下文对象.</param>
        protected QueryDao(DbContext dbc = null)
        {
            GenerateTableName = false;
            db = dbc;
        }

        /// <summary>
        /// 在字段前添加表名称则为 true，否则为 false.
        /// </summary>
        internal bool GenerateTableName { get; set; }

        /// <summary>
        /// 获取或设置表名称.
        /// </summary>
        internal string TableName { get; set; }

        /// <summary>
        /// 获取或设置数据库上下文.
        /// </summary>
        public DbContext db { get; set; }

        /// <summary>
        /// 获取指定的查询字段.
        /// </summary>
        /// <param name="name">字段名.</param>
        /// <returns></returns>
        public FieldDescription GetField(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (GenerateTableName && !string.IsNullOrEmpty(TableName))
                return td.Field(TableName, name);

            return td.Field(name);
        }

        /// <summary>
        /// 该数据访问器对应的实体类型.
        /// </summary>
        public abstract Type EntityType { get; }

        /// <summary>
        /// 获取所有字段的命令描述.
        /// </summary>
        public IDescription[] All => GetAllFields();

        /// <summary>
        /// 获取该查询访问器对应的表上下文对象（在调用此方法前应先设置 <see cref="TableName"/> 属性）.
        /// </summary>
        /// <returns></returns>
        internal IDbTableContext GetTableContext()
        {
            if (string.IsNullOrEmpty(TableName))
            {
                IDbTableContext tableContext = GetTableContext(null);
                TableName = tableContext.tableName;
                return tableContext;
            }
            return GetTableContext(TableName);
        }

        /// <summary>
        /// 获取该数据访问器对应的表上下文对象.
        /// </summary>
        /// <param name="name">表名称（为 null 则返回符合该查询访问器的第一个表上下文）.</param>
        /// <returns></returns>
        protected abstract IDbTableContext GetTableContext(string name);

        /// <summary>
        /// 获取所有字段的命令描述.
        /// </summary>
        /// <returns></returns>
        protected abstract IDescription[] GetAllFields();
    }
}
