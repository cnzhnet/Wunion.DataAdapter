using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 表示 Order By 排序方式的枚举类型。
    /// </summary>
    public enum OrderByMode
    {
        /// <summary>
        /// 表示升序。
        /// </summary>
        ASC,
        /// <summary>
        /// 表示降序
        /// </summary>
        DESC
    }

    /// <summary>
    /// 用于描述 ORDER BY 子句的对象类型。
    /// </summary>
    public class OrderByBlock : ParseDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.OrderByBlock"/> 的对象实例。
        /// </summary>
        public OrderByBlock()
        { }

        /// <summary>
        /// 获取或设置排序的字段。
        /// </summary>
        public FieldDescription Field
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置排序类型。
        /// </summary>
        public OrderByMode Sort
        {
            get;
            set;
        }
    }
}
