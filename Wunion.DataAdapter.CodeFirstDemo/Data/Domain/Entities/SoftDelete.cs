using System;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 用于实现支持软删除的实体.
    /// </summary>
    public interface ISoftDelete
    { 
        /// <summary>
        /// 是否已删除.
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// 删除日期.
        /// </summary>
        DateTime? DeletionDate { get; set; }
    }

    /// <summary>
    /// 定义软删除字段的实体基类.
    /// </summary>
    public abstract class SoftDelete : ISoftDelete
    {
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
        [TableField(DbType = GenericDbType.DateTime, Default = false, NotNull = true)]
        public DateTime? DeletionDate { get; set; }

        /// <summary>
        /// 创建一个软删除实体字段定义.
        /// </summary>
        protected SoftDelete()
        { }
    }
}
