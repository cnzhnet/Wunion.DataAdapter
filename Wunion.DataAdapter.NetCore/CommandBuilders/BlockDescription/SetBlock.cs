using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述 Set 子句的对象类型。
    /// </summary>
    public class SetBlock : ParseDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.SetBlock"/> 的对象实例。
        /// </summary>
        public SetBlock()
        {
            Expressions = new List<IDescription>();
        }

        /// <summary>
        /// 获取或设置 Set 后的表达式。
        /// </summary>
        public List<IDescription> Expressions
        {
            get;
            set;
        }
    }
}
