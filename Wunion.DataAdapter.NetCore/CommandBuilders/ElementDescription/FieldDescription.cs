using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述字段信息的对象类型。
    /// </summary>
    public class FieldDescription : OperatorDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.FieldDescription"/> 的对象实例。
        /// </summary>
        public FieldDescription()
        { }

        /// <summary>
        /// 获取或设置表名称或别名（多表查询时可需要）
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置字段名称。
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// 将字段 As 为一个指定的名称。
        /// </summary>
        /// <param name="Name">欲 AS 为的别名。</param>
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
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator ==(FieldDescription field, object item2)
        {
            return exp.Create(field, '=', item2);
        }

        /// <summary>
        /// 创建不等于运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static OperatorDescription operator !=(FieldDescription field, object item2)
        {
            if (object.ReferenceEquals(item2, null))
                return Fun.IsNotNull(field);
            else
                return exp.Create(field, exp.NotEqual, item2);
        }

        /// <summary>
        /// 创建大于关系运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator >(FieldDescription field, object item2)
        {
            return exp.Create(field, '>', item2);
        }

        /// <summary>
        /// 创建大于等于关系运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator >=(FieldDescription field, object item2)
        {
            return exp.Create(field, exp.GreaterOrEqual, item2);
        }

        /// <summary>
        /// 创建小于关系运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <(FieldDescription field, object item2)
        {
            return exp.Create(field, '<', item2);
        }

        /// <summary>
        /// 创建小于等于关系运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator <=(FieldDescription field, object item2)
        {
            return exp.Create(field, exp.LessOrEqual, item2);
        }

        /// <summary>
        /// 创建加法运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator +(FieldDescription field, object item2)
        {
            return exp.Create(field, '+', item2);
        }

        /// <summary>
        /// 创建减法运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator -(FieldDescription field, object item2)
        {
            return exp.Create(field, '-', item2);
        }

        /// <summary>
        /// 创建乘法运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator *(FieldDescription field, object item2)
        {
            return exp.Create(field, '*', item2);
        }

        /// <summary>
        /// 创建除法运算表达式.
        /// </summary>
        /// <param name="field">左操作数对象.</param>
        /// <param name="item2">右操作数对象.</param>
        /// <returns></returns>
        public static ExpDescription operator /(FieldDescription field, object item2)
        {
            return exp.Create(field, '/', item2);
        }

        /// <summary>
        /// 创建逻辑与运算表达式.
        /// </summary>
        /// <param name="left">左操作数对象.</param>
        /// <param name="right">右操作数对象.</param>
        /// <returns></returns>
        public static LogicAndDescription operator &(FieldDescription left, object right)
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
        public static LogicOrDescription operator |(FieldDescription left, object right)
        {
            LogicOrDescription logicOr = new LogicOrDescription();
            logicOr.LeftElement = left;
            logicOr.RightElement = right;
            return logicOr;
        }

        #endregion
    }

    /// <summary>
    /// 用于字段信息描述对象的快捷创建器。
    /// </summary>
    public class td
    {
        /// <summary>
        /// 创建一个字段信息描述对象。
        /// </summary>
        /// <param name="name">字段名称。</param>
        /// <returns></returns>
        public static FieldDescription Field(string name)
        {
            return Field(string.Empty, name);
        }

        /// <summary>
        /// 创建一个字段信息描述对象。
        /// </summary>
        /// <param name="tbname">所属表的名称或别名（多表查询时需要）。</param>
        /// <param name="fieldname">字段名称。</param>
        /// <returns></returns>
        public static FieldDescription Field(string tbname, string fieldname)
        {
            FieldDescription fd = new FieldDescription();
            fd.TableName = tbname;
            fd.FieldName = fieldname;
            return fd;
        }

        /// <summary>
        /// 将指定的值在查询中 AS 为一个字段。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="AsName">AS为的字段名称。</param>
        /// <returns></returns>
        public static AsElementDecsription As(object value, string AsName)
        {
            AsElementDecsription elem = new AsElementDecsription();
            elem.AsName = AsName;
            elem.Objective = value;
            return elem;
        }
    }
}
