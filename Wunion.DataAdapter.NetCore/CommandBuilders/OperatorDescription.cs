using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 表示支持关系运算及逻辑运算的描述对象基础类型
    /// </summary>
    public abstract class OperatorDescription : ParseDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.OperatorDescription"/> 的对象实例.
        /// </summary>
        protected OperatorDescription() { }

        /// <summary>
        /// 创建运算的优先级分组.
        /// </summary>
        public GroupDescription Group
        {
            get { return new GroupDescription(this); }
        }

        /// <summary>
        /// 该达式的 IN 函数支持.
        /// </summary>
        /// <param name="select">IN 函数的条件查询.</param>
        /// <returns></returns>
        public FunDescription In(IDescription select)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "IN";
            List<object> buf = new List<object>();
            buf.Add(this);
            buf.Add(select);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// 该达式的 IN 函数支持.
        /// </summary>
        /// <param name="values">IN 函数的条件值.</param>
        /// <returns></returns>
        public FunDescription In(object[] values)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "IN";
            List<object> buf = new List<object>();
            buf.Add(this);
            buf.AddRange(values);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// 表达式的 NotIn 函数支持.
        /// </summary>
        /// <param name="select">条件查询.</param>
        /// <returns></returns>
        public FunDescription NotIn(IDescription select)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "NOTIN";
            List<object> buf = new List<object>();
            buf.Add(this);
            buf.Add(select);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// 表达式的 NotIn 函数支持.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public FunDescription NotIn(object[] values)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "NOTIN";
            List<object> buf = new List<object>();
            buf.Add(this);
            buf.AddRange(values);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// IS NULL 函数。
        /// </summary>
        /// <returns></returns>
        public FunDescription IsNull()
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "ISNULL";
            f.Parameter = this;
            return f;
        }

        /// <summary>
        /// IS NOT NULL 函数。
        /// </summary>
        /// <returns></returns>
        public FunDescription IsNotNull()
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "ISNOTNULL";
            f.Parameter = this;
            return f;
        }

        /// <summary>
        /// 创建一个 LIKE 子句描述。
        /// </summary>
        /// <param name="content">LIKE 子句中的内容部分（请不要指定 ' 及 % 或 * 号之类的信息）。</param>
        /// <param name="match">匹配模式。</param>
        /// <returns></returns>
        public LikeDescription Like(object content, LikeMatch match)
        {
            LikeDescription lk = new LikeDescription();
            lk.Field = this;
            lk.Content = content;
            lk.Match = match;
            return lk;
        }

        #region 运算符重载

        /// <summary>
        /// 创建等于运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator ==(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, '=', item2);
        }

        /// <summary>
        /// 创建不等于运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator !=(OperatorDescription item1, object item2)
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
        public static ExpDescription operator >(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, '>', item2);
        }

        /// <summary>
        /// 创建大于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator >=(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, exp.GreaterOrEqual, item2);
        }

        /// <summary>
        /// 创建小于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, '<', item2);
        }

        /// <summary>
        /// 创建小于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <=(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, exp.LessOrEqual, item2);
        }

        /// <summary>
        /// 创建加法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator +(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, '+', item2);
        }

        /// <summary>
        /// 创建减法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator -(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, '-', item2);
        }

        /// <summary>
        /// 创建乘法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator *(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, '*', item2);
        }

        /// <summary>
        /// 创建除法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator /(OperatorDescription item1, object item2)
        {
            return exp.Create(item1, '/', item2);
        }

        /// <summary>
        /// 创建逻辑与运算表达式.
        /// </summary>
        /// <param name="left">左操作数对象.</param>
        /// <param name="right">右操作数对象.</param>
        /// <returns></returns>
        public static LogicAndDescription operator &(OperatorDescription left, object right)
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
        public static LogicOrDescription operator |(OperatorDescription left, object right)
        {
            LogicOrDescription logicOr = new LogicOrDescription();
            logicOr.LeftElement = left;
            logicOr.RightElement = right;
            return logicOr;
        }

        #endregion
    }
}
