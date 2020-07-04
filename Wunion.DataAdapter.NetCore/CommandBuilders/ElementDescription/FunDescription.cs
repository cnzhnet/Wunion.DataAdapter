using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 函数的描述对象类型。
    /// </summary>
    public class FunDescription : OperatorDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.FunDescription"/> 的对象实例。
        /// </summary>
        public FunDescription()
        { }

        /// <summary>
        /// 函数的参数。
        /// </summary>
        public object Parameter
        {
            get;
            set;
        }

        /// <summary>
        /// 函数名称。
        /// </summary>
        public string FunctionName
        {
            get;
            set;
        }

        /// <summary>
        /// As 别名（如：SELECT [MONEY] / 1000 AS [K_MONEY] ....，中的 K_MONEY）。
        /// </summary>
        /// <param name="Name"></param>
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
        public static OperatorDescription operator ==(FunDescription item1, object item2)
        {
            return exp.Create(item1, '=', item2);
        }

        /// <summary>
        /// 创建不等于运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator !=(FunDescription item1, object item2)
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
        public static ExpDescription operator >(FunDescription item1, object item2)
        {
            return exp.Create(item1, '>', item2);
        }

        /// <summary>
        /// 创建大于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator >=(FunDescription item1, object item2)
        {
            return exp.Create(item1, exp.GreaterOrEqual, item2);
        }

        /// <summary>
        /// 创建小于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <(FunDescription item1, object item2)
        {
            return exp.Create(item1, '<', item2);
        }

        /// <summary>
        /// 创建小于等于关系运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <=(FunDescription item1, object item2)
        {
            return exp.Create(item1, exp.LessOrEqual, item2);
        }

        /// <summary>
        /// 创建加法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator +(FunDescription item1, object item2)
        {
            return exp.Create(item1, '+', item2);
        }

        /// <summary>
        /// 创建减法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator -(FunDescription item1, object item2)
        {
            return exp.Create(item1, '-', item2);
        }

        /// <summary>
        /// 创建乘法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator *(FunDescription item1, object item2)
        {
            return exp.Create(item1, '*', item2);
        }

        /// <summary>
        /// 创建除法运算表达式.
        /// </summary>
        /// <param name="item1">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator /(FunDescription item1, object item2)
        {
            return exp.Create(item1, '/', item2);
        }

        /// <summary>
        /// 创建逻辑与运算表达式.
        /// </summary>
        /// <param name="left">左操作数对象.</param>
        /// <param name="right">右操作数对象.</param>
        /// <returns></returns>
        public static LogicAndDescription operator &(FunDescription left, object right)
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
        public static LogicOrDescription operator |(FunDescription left, object right)
        {
            LogicOrDescription logicOr = new LogicOrDescription();
            logicOr.LeftElement = left;
            logicOr.RightElement = right;
            return logicOr;
        }

        #endregion
    }

    /// <summary>
    /// 表示日期部份的枚举。
    /// </summary>
    public enum DateInterval
    {
        Year,
        Quarter,
        Month,
        DayOfYear,
        Day,
        Week,
        WeekDay,
        Hour,
        Minute,
        Second,
        Millisecond
    }

    /// <summary>
    /// 函数描述对象的快捷创建器。
    /// </summary>
    public static class Fun
    {
        /// <summary>
        /// 去重复值的字段列表。
        /// </summary>
        /// <param name="parameter">字段列表（单个或多个字段，注：某些数据库可能不支持多字段去重复值）。</param>
        /// <returns></returns>
        public static FunDescription Distinct(params FieldDescription[] parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "DISTINCT";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 统计函数。
        /// </summary>
        /// <param name="parameter">函数参数。</param>
        /// <returns></returns>
        public static FunDescription Count(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "COUNT";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 求和函数。
        /// </summary>
        /// <param name="parameter">函数参数。</param>
        /// <returns></returns>
        public static FunDescription Sum(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "SUM";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 求平均值函数。
        /// </summary>
        /// <param name="parameter">函数参数。</param>
        /// <returns></returns>
        public static FunDescription Avg(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "AVG";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 最大值函数。
        /// </summary>
        /// <param name="parameter">函数参数。</param>
        /// <returns></returns>
        public static FunDescription Max(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "MAX";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 最小值函数。
        /// </summary>
        /// <param name="parameter">函数参数。</param>
        /// <returns></returns>
        public static FunDescription Min(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "MIN";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 获取当前日期时间函数。
        /// </summary>
        /// <param name="parameter">函数参数。</param>
        /// <returns></returns>
        public static FunDescription Now(object parameter = null)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "NOW";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 返回给定日期整型部份的数据。
        /// </summary>
        /// <param name="interval">日期时间中对应的部份。
        /// <para>SQLite不支持此函数。</para>
        /// </param>
        /// <param name="thedate">日期时间。</param>
        /// <returns></returns>
        public static FunDescription DatePart(DateInterval interval, object thedate)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "DATEPART";
            f.Parameter = new object[] { interval, thedate };
            return f;
        }

        /// <summary>
        /// 获取长度函数。
        /// </summary>
        /// <param name="parameter">函数参数。</param>
        /// <returns></returns>
        public static FunDescription Len(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "LEN";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// 字符串截取函数。
        /// </summary>
        /// <param name="str">要截取的字符串。</param>
        /// <param name="start">开始截取的位置。</param>
        /// <param name="length">截取数量。</param>
        /// <returns></returns>
        public static FunDescription Substring(object expression, int start, int length)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "SUBSTRING";
            f.Parameter = new object[] { expression, start, length };
            return f;
        }

        /// <summary>
        /// IN 函数。
        /// </summary>
        /// <param name="field">字段信息。</param>
        /// <param name="select">嵌套子查询。</param>
        /// <returns></returns>
        public static FunDescription In(FieldDescription field, IDescription select)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "IN";
            List<object> buf = new List<object>();
            buf.Add(field);
            buf.Add(select);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// IN 函数。
        /// </summary>
        /// <param name="field">字段信息。</param>
        /// <param name="values">In函数的值。</param>
        /// <returns></returns>
        public static FunDescription In(FieldDescription field, object[] values)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "IN";
            List<object> buf = new List<object>();
            buf.Add(field);
            buf.AddRange(values);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// NOT IN 函数。
        /// </summary>
        /// <param name="field">字段信息。</param>
        /// <param name="select">嵌套子查询。</param>
        /// <returns></returns>
        public static FunDescription NotIn(FieldDescription field, IDescription select)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "NOTIN";
            List<object> buf = new List<object>();
            buf.Add(field);
            buf.Add(select);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// NOT IN 函数。
        /// </summary>
        /// <param name="field">字段信息。</param>
        /// <param name="values">NOT IN 函数的值。</param>
        /// <returns></returns>
        public static FunDescription NotIn(FieldDescription field, object[] values)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "NOTIN";
            List<object> buf = new List<object>();
            buf.Add(field);
            buf.AddRange(values);
            f.Parameter = buf.ToArray();
            return f;
        }

        /// <summary>
        /// IS NULL 函数。
        /// </summary>
        /// <param name="parameter">参数（一般为字段信息）。</param>
        /// <returns></returns>
        public static FunDescription IsNull(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "ISNULL";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// IS NOT NULL 函数。
        /// </summary>
        /// <param name="parameter">参数（一般为字段信息）。</param>
        /// <returns></returns>
        public static FunDescription IsNotNull(object parameter)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "ISNOTNULL";
            f.Parameter = parameter;
            return f;
        }

        /// <summary>
        /// BETWEEN...AND 子句。
        /// </summary>
        /// <param name="fieldOrexp">字段信息或表达式。</param>
        /// <param name="start">开始值。</param>
        /// <param name="end">结束值。</param>
        /// <returns></returns>
        public static FunDescription BetweenAnd(object fieldOrexp, object start, object end)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "BETWEENAND";
            f.Parameter = new object[] { fieldOrexp, start, end };
            return f;
        }

        /// <summary>
        /// NOT BETWEEN...AND 子句。
        /// </summary>
        /// <param name="fieldOrexp">字段信息或表达式。</param>
        /// <param name="start">开始值。</param>
        /// <param name="end">结束值。</param>
        /// <returns></returns>
        public static FunDescription NotBetweenAnd(object fieldOrexp, object start, object end)
        {
            FunDescription f = new FunDescription();
            f.FunctionName = "NOTBETWEENAND";
            f.Parameter = new object[] { fieldOrexp, start, end };
            return f;
        }
    }
}
