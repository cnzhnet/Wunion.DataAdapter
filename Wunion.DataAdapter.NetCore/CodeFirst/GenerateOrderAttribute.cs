using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于表示数据表或字段的生成顺序.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GenerateOrderAttribute : Attribute
    {
        /// <summary>
        /// 创建一个 <see cref="GenerateOrderAttribute"/> 的地象实例.
        /// </summary>
        /// <param name="index">顺序的索引值（从 0 开始）.</param>
        public GenerateOrderAttribute(int index)
        {
            Index = index;
        }

        /// <summary>
        /// 顺序的索引值（从 0 开始）.
        /// </summary>
        public int Index { get; set; }
    }
}
