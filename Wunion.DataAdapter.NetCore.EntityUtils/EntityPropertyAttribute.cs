using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 表示实体的属性与表字段的映射信息。
    /// </summary>
    public class EntityPropertyAttribute : Attribute
    {
        /// <summary>
        /// 创建一个 <see cref="EntityPropertyAttribute"/> 的对象实例.
        /// </summary>
        public EntityPropertyAttribute()
        {
            DefaultValue = null;
            AllowNull = true;
            PrimaryKey = false;
            IsIdentity = false;
        }

        /// <summary>
        /// 获取或属性的默认值.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 获取或设置该值性是否允许空值.
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// 获取或设置属性是否为主键.
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 获取或设置是否为自增长字段.
        /// </summary>
        public bool IsIdentity { get; set; }
    }
}
