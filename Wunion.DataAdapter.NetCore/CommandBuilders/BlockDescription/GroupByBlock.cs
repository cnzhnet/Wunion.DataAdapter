using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述分组子句的对象类型。
    /// </summary>
    public class GroupByBlock : ParseDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.GroupByBlock"/> 的对象实例。
        /// </summary>
        public GroupByBlock()
        {
            Fields = new List<FieldDescription>();
        }

        /// <summary>
        /// 获取分组所需的字段类型。
        /// </summary>
        public List<FieldDescription> Fields
        {
            get;
            set;
        }
    }
}
