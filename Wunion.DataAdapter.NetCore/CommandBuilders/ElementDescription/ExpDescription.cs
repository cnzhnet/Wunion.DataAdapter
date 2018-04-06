using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述命令中的表达式的对象类型。
    /// </summary>
    public class ExpDescription : OperatorDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.ExpDescription"/> 的对象类型。
        /// </summary>
        public ExpDescription()
        {
            ExpElements = new List<object>();
        }

        /// <summary>
        /// 获取或设置表达式中的各个元素集合。
        /// </summary>
        public List<object> ExpElements
        {
            get;
            set;
        }

        /// <summary>
        /// 对该表达式使用逻辑非运算
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

        /// <summary>
        /// 表达式的 As 别名（如：SELECT [MONEY] / 1000 AS [K_MONEY] ....，中的 K_MONEY）。
        /// </summary>
        /// <param name="Name">别名名称。</param>
        /// <returns></returns>
        public AsElementDecsription As(string Name)
        {
            AsElementDecsription elem = new AsElementDecsription();
            elem.AsName = Name;
            elem.Objective = this;
            return elem;
        }

        #region 运算符重载

        /// <summary>
        /// 创建等于运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator ==(ExpDescription item1, object item2)
        {
            return exp.Create(item1, '=', item2);
        }

        /// <summary>
        /// 创建不等于运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator !=(ExpDescription item1, object item2)
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
        public static ExpDescription operator >(ExpDescription item1, object item2)
        {
            return exp.Create(item1, '>', item2);
        }

        /// <summary>
        /// 创建大于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator >=(ExpDescription item1, object item2)
        {
            return exp.Create(item1, exp.GreaterOrEqual, item2);
        }

        /// <summary>
        /// 创建小于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <(ExpDescription item1, object item2)
        {
            return exp.Create(item1, '<', item2);
        }

        /// <summary>
        /// 创建小于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <=(ExpDescription item1, object item2)
        {
            return exp.Create(item1, exp.LessOrEqual, item2);
        }

        /// <summary>
        /// 创建加法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator +(ExpDescription item1, object item2)
        {
            return exp.Create(item1, '+', item2);
        }

        /// <summary>
        /// 创建减法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator -(ExpDescription item1, object item2)
        {
            return exp.Create(item1, '-', item2);
        }

        /// <summary>
        /// 创建乘法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator *(ExpDescription item1, object item2)
        {
            return exp.Create(item1, '*', item2);
        }

        /// <summary>
        /// 创建除法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator /(ExpDescription item1, object item2)
        {
            return exp.Create(item1, '/', item2);
        }

        /// <summary>
        /// 创建逻辑与运算表达式.
        /// </summary>
        /// <param name="left">左操作数对象.</param>
        /// <param name="right">右操作数对象.</param>
        /// <returns></returns>
        public static LogicAndDescription operator &(ExpDescription left, object right)
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
        public static LogicOrDescription operator |(ExpDescription left, object right)
        {
            LogicOrDescription logicOr = new LogicOrDescription();
            logicOr.LeftElement = left;
            logicOr.RightElement = right;
            return logicOr;
        }

        #endregion
    }

    /// <summary>
    /// LIKE 子句的匹配模式。
    /// </summary>
    public enum LikeMatch
    {
        /// <summary>
        /// 左匹配。
        /// </summary>
        Left = 0x0,
        /// <summary>
        /// 中匹配。
        /// </summary>
        Center = 0x1,
        /// <summary>
        /// 右匹配。
        /// </summary>
        Right = 0x2
    }

    /// <summary>
    /// 用于描述 LIKE 子句的对象类型。
    /// </summary>
    public class LikeDescription : ParseDescription
    {
        /// <summary>
        /// LIKE 子句的字段。
        /// </summary>
        public IDescription Field
        {
            get;
            set;
        }

        /// <summary>
        /// LIKE 子的的内容部份。
        /// </summary>
        public object Content
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置 LIKE 子句的匹配模式。
        /// </summary>
        public LikeMatch Match
        {
            get;
            set;
        }

        /// <summary>
        /// 对该表达式使用逻辑非运算
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
        /// 创建逻辑与运算表达式.
        /// </summary>
        /// <param name="left">左操作数对象.</param>
        /// <param name="right">右操作数对象.</param>
        /// <returns></returns>
        public static LogicAndDescription operator &(LikeDescription left, object right)
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
        public static LogicOrDescription operator |(LikeDescription left, object right)
        {
            LogicOrDescription logicOr = new LogicOrDescription();
            logicOr.LeftElement = left;
            logicOr.RightElement = right;
            return logicOr;
        }
        #endregion
    }

    /// <summary>
    /// 用于描述表达式信息的对象。
    /// </summary>
    public static class exp
    {
        /// <summary>
        /// AND 运算符。
        /// </summary>
        public static char And
        {
            get { return '&'; }
        }

        /// <summary>
        /// OR 运算符。
        /// </summary>
        public static char Or
        {
            get { return '|'; }
        }

        /// <summary>
        /// 不等于。
        /// </summary>
        public static char NotEqual
        {
            get { return '≠'; }
        }

        /// <summary>
        /// 小于等于。
        /// </summary>
        public static char LessOrEqual
        {
            get { return '≤'; }
        }

        /// <summary>
        /// 大于等于。
        /// </summary>
        public static char GreaterOrEqual
        {
            get { return '≥'; }
        }

        /// <summary>
        /// 创建一个表达式描述。
        /// </summary>
        /// <param name="elements">表达式中的各个元素集合。</param>
        /// <returns></returns>
        public static ExpDescription Create(params object[] elements)
        {
            ExpDescription e = new ExpDescription();
            e.ExpElements.AddRange(elements);
            return e;
        }

        /// <summary>
        /// 创建一个 LIKE 子句描述。
        /// </summary>
        /// <param name="field">LIKE 子句中的字段。</param>
        /// <param name="content">LIKE 子句中的内容部分（请不要指定 ' 及 % 或 * 号之类的信息）。</param>
        /// <param name="match">匹配模式。</param>
        /// <returns></returns>
        public static LikeDescription Like(FieldDescription field, object content, LikeMatch match)
        {
            LikeDescription lk = new LikeDescription();
            lk.Field = field;
            lk.Content = content;
            lk.Match = match;
            return lk;
        }
    }
}
