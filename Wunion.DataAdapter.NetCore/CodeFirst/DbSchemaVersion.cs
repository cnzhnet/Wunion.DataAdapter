using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.Querying;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 表示数据库架构的版本信息.
    /// </summary>
    public class DbSchemaVersion
    {
        /// <summary>
        /// 创建一个 <see cref="DbSchemaVersion"/> 的对象实例.
        /// </summary>
        public DbSchemaVersion()
        { }

        /// <summary>
        /// 表示该版本的名称描述.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 32, NotNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// 表示版本号.
        /// </summary>
        [TableField(DbType = GenericDbType.Int, NotNull = true, PrimaryKey = true)]
        public int Version { get; set; }

        /// <summary>
        /// 表示该版本的创建日期.
        /// </summary>
        [TableField(DbType = GenericDbType.DateTime, NotNull = true)]
        public DateTime Creation { get; set; }
    }

    /// <summary>
    /// 数据库架构查询访问器.
    /// </summary>
    public class DbSchemaVersionDao : QueryDao
    {
        /// <summary>
        /// 创建一个数据库架构查询访问器.
        /// </summary>
        /// <param name="dbc">数据库上下文对象.</param>
        public DbSchemaVersionDao(DbContext dbc) : base(dbc)
        { }

        /// <summary>
        /// 创建一个数据库架构查询访问器.
        /// </summary>
        public DbSchemaVersionDao() : base(null)
        { }

        /// <summary>
        /// 获取该查询访问器对应的实体类型.
        /// </summary>
        public override Type EntityType => typeof(DbSchemaVersion);

        /// <summary>
        /// 表示该版本的名称描述.
        /// </summary>
        public FieldDescription Name => GetField("Name");

        /// <summary>
        /// 表示版本号.
        /// </summary>
        public FieldDescription Version => GetField("Version");

        /// <summary>
        /// 表示该版本的创建日期.
        /// </summary>
        public FieldDescription Creation => GetField("Creation");

        /// <summary>
        /// 获取该数据访问器对应的表上下文对象.
        /// </summary>
        /// <param name="name">表名称（为 null 则返回符合该查询访问器的第一个表上下文）.</param>
        /// <returns></returns>
        protected override IDbTableContext GetTableContext(string name)
        {
            if (string.IsNullOrEmpty(name))
                return db?.TableDeclaration<DbSchemaVersion>(DbContext.SCHEMA_VERSION_NAME);
            return db?.TableDeclaration<DbSchemaVersion>(name);
        }

        /// <summary>
        /// 获取所有字段.
        /// </summary>
        /// <returns></returns>
        protected override IDescription[] GetAllFields()
        {
            return new IDescription[] { Name, Version, Creation };
        }
    }
}
