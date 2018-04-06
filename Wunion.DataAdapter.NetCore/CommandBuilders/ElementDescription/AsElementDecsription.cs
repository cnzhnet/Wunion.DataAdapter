using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述一个 As 元素信息的对象类型。
    /// </summary>
    public class AsElementDecsription : ParseDescription
    {
        /// <summary>
        /// 创建 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.AsElementDecsription"/> 的对象实例。
        /// </summary>
        public AsElementDecsription()
        { }

        /// <summary>
        /// 获取或设置别名名称。
        /// </summary>
        public string AsName
        {
            get;
            set;
        }

        /// <summary>
        /// 欲将其以AS别名描述的对象（可以是其它元素或一个任意的值）。
        /// </summary>
        public object Objective
        {
            get;
            set;
        }
    }
}
