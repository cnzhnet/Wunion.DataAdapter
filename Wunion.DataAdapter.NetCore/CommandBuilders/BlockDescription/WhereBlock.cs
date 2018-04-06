using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述 WHERE 子句段的对象类型。
    /// </summary>
    public class WhereBlock : ParseDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.WhereBlock"/> 的对象类型。
        /// </summary>
        public WhereBlock()
        {
            Content = new List<object>();
        }

        /// <summary>
        /// 获取或设置 WHERE 子句的内容。
        /// </summary>
        public List<object> Content
        {
            get;
            set;
        }
    }
}
