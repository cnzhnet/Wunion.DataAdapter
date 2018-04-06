using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 表示实体类与数据表的映射信息.
    /// </summary>
    public class EntityTableAttribute : Attribute
    {
        /// <summary>
        /// 创建一个 <see cref="EntityTableAttribute"/> 的对象实例.
        /// </summary>
        public EntityTableAttribute() { }

        /// <summary>
        /// 获取或设置实体对应表的表名称.
        /// </summary>
        public string TableName { get; set; }
    }
}
