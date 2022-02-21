using System;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于标记字段的外键映射信息.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// 当删除父表中的记录时允许删除子表中的记录，当更新父表中的记录时时允许交叉更新子表.
        /// </summary>
        public const string ACTION_CASCADE = "CASCADE";
        /// <summary>
        /// 当删除或更新父表中的记录值时，将子表中的记录值设置为 NULL （前提是该字段支持 NULL 值）.
        /// </summary>
        public const string ACTION_SET_NULL = "SET NULL";
        /// <summary>
        /// 拒绝删除或更新子表中的记录.
        /// </summary>
        public const string ACTION_RESTRICT = "RESTRICT";

        /// <summary>
        /// 外键映射的表名称.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 从表的映射字段名称.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 当父表中的记录被删除时的行为.
        /// </summary>
        public string OnDeleteAction { get; set; }

        /// <summary>
        /// 当父表中的记录被更新时的行为.
        /// </summary>
        public string OnUpdateAction { get; set; }

        /// <summary>
        /// 创建一个 <see cref="ForeignKeyAttribute"/> 的对象实例.
        /// </summary>
        public ForeignKeyAttribute()
        {
            OnDeleteAction = ACTION_CASCADE;
            OnUpdateAction = ACTION_CASCADE;
        }

        /// <summary>
        /// 创建外键约束描述对象.
        /// </summary>
        /// <returns></returns>
        public DbForeignKey CreateForeignKey()
        {
            if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(Field))
                return null;
            return new DbForeignKey(TableName, Field, OnUpdateAction, OnDeleteAction);
        }
    }
}
