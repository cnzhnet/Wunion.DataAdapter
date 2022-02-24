using System;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 定义包含记录写入（创建及最后修改）日期字段的实体基类.
    /// </summary>
    public abstract class WriteDateTime
    {
        /// <summary>
        /// 创建日期.
        /// </summary>
        [GenerateOrder(160)]
        [TableField(DbType = GenericDbType.DateTime)]
        public DateTime? Creation { get; set; }

        /// <summary>
        /// 最后修改日期.
        /// </summary>
        [GenerateOrder(161)]
        [TableField(DbType = GenericDbType.DateTime)]
        public DateTime? LastModified { get; set; }

        protected WriteDateTime()
        { }
    }

    /// <summary>
    /// 同时定义记录写入（创建及最后修改）日期及软删除字段的实体基类.
    /// </summary>
    public abstract class WriteDateSoftDelete : WriteDateTime, ISoftDelete
    {
        protected WriteDateSoftDelete()
        { }

        /// <summary>
        /// 是否已删除.
        /// </summary>
        [GenerateOrder(200)]
        [TableField(DbType = GenericDbType.Boolean, Default = false, NotNull = true)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除日期.
        /// </summary>
        [GenerateOrder(201)]
        [TableField(DbType = GenericDbType.DateTime)]
        public DateTime? DeletionDate { get; set; }
    }
}
