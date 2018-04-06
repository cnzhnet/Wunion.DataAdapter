using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 表示命令中的逻辑运算表达式描述对象.
    /// </summary>
    public abstract class LogicDescription : OperatorDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.LogicDescription"/> 的对象实例.
        /// </summary>
        protected LogicDescription()
        { }

        /// <summary>
        /// 获取或设置逻辑运算的左操元素.
        /// </summary>
        public object LeftElement { get; set; }

        /// <summary>
        /// 获取或设置逻辑运算的右操作元素.
        /// </summary>
        public object RightElement { get; set; }
    }

    /// <summary>
    /// 表示命令中的逻辑与运算表达式描述对象.
    /// </summary>
    public class LogicAndDescription : LogicDescription
    {
        /// <summary>
        /// 创建一个逻辑与运算表达式描述对象的实例.
        /// </summary>
        public LogicAndDescription()
        { }
    }

    /// <summary>
    /// 表示命令中的逻辑或运算表达式描述对象.
    /// </summary>
    public class LogicOrDescription : LogicDescription
    {
        /// <summary>
        /// 创建一个逻辑或运算表达式描述对象的实例.
        /// </summary>
        public LogicOrDescription()
        { }
    }

    /// <summary>
    /// 表示命令中的逻辑非运算表达式描述对象.
    /// </summary>
    public class LogicNotDescription : ParseDescription
    {
        /// <summary>
        /// 创建一个逻辑非运算表达式描述对象的实例.
        /// </summary>
        public LogicNotDescription()
        { }

        /// <summary>
        /// 获取或设置要应用逻辑非运算的表达式.
        /// </summary>
        public IDescription Expression { get; set; }
    }
}
