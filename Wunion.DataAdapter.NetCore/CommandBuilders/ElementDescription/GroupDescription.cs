using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 表示元素优先级分组（即命令中的括号分组）的描述对象.
    /// </summary>
    public class GroupDescription : OperatorDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.GroupDescription"/> 的对象实例.
        /// </summary>
        public GroupDescription()
        { }

        /// <summary>
        /// 创建一个元素优先级分组描述对象.
        /// </summary>
        /// <param name="element">该分组内包含的元素对象.</param>
        public GroupDescription(IDescription element)
        {
            Content = element;
        }

        /// <summary>
        /// 获取或设置分组内容元素对象.
        /// </summary>
        public IDescription Content { get; set; }

        /// <summary>
        /// 对该表分组使用逻辑非运算
        /// </summary>
        public LogicNotDescription Not
        {
            get
            {
                LogicNotDescription logicNot = new LogicNotDescription();
                logicNot.Expression = this;
                return logicNot;
            }
        }

        #region 运算符重载

        /// <summary>
        /// 创建等于运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator ==(GroupDescription item1, object item2)
        {
            return exp.Create(item1, '=', item2);
        }

        /// <summary>
        /// 创建不等于运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator !=(GroupDescription item1, object item2)
        {
            if (object.ReferenceEquals(item2, null))
                return Fun.IsNotNull(item1);
            else
                return exp.Create(item1, exp.NotEqual, item2);
        }

        /// <summary>
        /// 创建大于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator >(GroupDescription item1, object item2)
        {
            return exp.Create(item1, '>', item2);
        }

        /// <summary>
        /// 创建大于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator >=(GroupDescription item1, object item2)
        {
            return exp.Create(item1, exp.GreaterOrEqual, item2);
        }

        /// <summary>
        /// 创建小于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <(GroupDescription item1, object item2)
        {
            return exp.Create(item1, '<', item2);
        }

        /// <summary>
        /// 创建小于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <=(GroupDescription item1, object item2)
        {
            return exp.Create(item1, exp.LessOrEqual, item2);
        }

        /// <summary>
        /// 创建加法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator +(GroupDescription item1, object item2)
        {
            return exp.Create(item1, '+', item2);
        }

        /// <summary>
        /// 创建减法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator -(GroupDescription item1, object item2)
        {
            return exp.Create(item1, '-', item2);
        }

        /// <summary>
        /// 创建乘法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator *(GroupDescription item1, object item2)
        {
            return exp.Create(item1, '*', item2);
        }

        /// <summary>
        /// 创建除法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator /(GroupDescription item1, object item2)
        {
            return exp.Create(item1, '/', item2);
        }

        /// <summary>
        /// 创建逻辑与运算表达式.
        /// </summary>
        /// <param name="left">左操作数对象.</param>
        /// <param name="right">右操作数对象.</param>
        /// <returns></returns>
        public static LogicAndDescription operator &(GroupDescription left, object right)
        {
            LogicAndDescription logicAnd = new LogicAndDescription();
            logicAnd.LeftElement = left;
            logicAnd.RightElement = right;
            return logicAnd;
        }

        /// <summary>
        /// 创建逻辑或运算表达式.
        /// </summary>
        /// <param name="left">左操作数对象.</param>
        /// <param name="right">右操作数对象.</param>
        /// <returns></returns>
        public static LogicOrDescription operator |(GroupDescription left, object right)
        {
            LogicOrDescription logicOr = new LogicOrDescription();
            logicOr.LeftElement = left;
            logicOr.RightElement = right;
            return logicOr;
        }

        #endregion
    }
}
