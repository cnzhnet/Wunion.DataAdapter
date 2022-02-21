using System;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于映身数据库与实体的关系映射.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableFieldAttribute : Attribute
    {
        /// <summary>
        /// 字段名.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段数据类型
        /// </summary>
        public GenericDbType DbType { get; set; }

        /// <summary>
        /// 默认值.
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        /// 是否为主键.
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 非空字段.
        /// </summary>
        public bool NotNull { get; set; }

        /// <summary>
        /// 是否唯一.
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// 字段长度.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 该字段的值类型转换器.
        /// </summary>
        public Type ValueConverter { get; set; }
    }
}
