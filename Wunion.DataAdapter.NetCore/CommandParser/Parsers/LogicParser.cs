using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 表示逻辑运算表达式的解释器.
    /// </summary>
    public abstract class LogicParser : ParserBase
    {
        /// <summary>
        /// 创建一个逻辑运算表达式的解释器对象实例.
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        protected LogicParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释逻辑运算的操作元素.
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <param name="elemObject">左或右操作元素对象.</param>
        /// <returns></returns>
        protected string ParseElementObject(ref List<IDbDataParameter> DbParameters, object elemObject)
        {
            if (elemObject == null || elemObject == DBNull.Value)
                throw new NotSupportedException("Can not use null objects for logical expression.");
            IDescription descr = elemObject as IDescription;
            string paramName = "u_LogicParam";
            if (descr == null)
            {
                IDbDataParameter p = Adapter.CreateDbParameter(paramName, elemObject);
                AddDbParameter(ref DbParameters, p);
                return string.Format(" {0}", p.ParameterName);
            }
            descr.DescriptionParserAdapter = Adapter;
            string buffer = descr.GetParser().Parsing(ref DbParameters);
            if (buffer[0] == (char)0x20)
                return buffer;
            else
                return string.Format(" {0}", buffer);
        }
    }

    /// <summary>
    /// 逻辑与运算表达式的解释器（一般通用）.
    /// </summary>
    public class LogicAndParser : LogicParser
    {
        /// <summary>
        /// 实例化一个逻辑与运算表达式解释器
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public LogicAndParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释逻辑与运算表达式.
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            LogicAndDescription logicAnd = (LogicAndDescription)this.Description;
            StringBuilder cBuffer = new StringBuilder();
            cBuffer.Append(ParseElementObject(ref DbParameters, logicAnd.LeftElement));
            cBuffer.Append(" AND");
            cBuffer.Append(ParseElementObject(ref DbParameters, logicAnd.RightElement));
            return cBuffer.ToString();
        }
    }

    /// <summary>
    /// 逻辑或运算表达式的解释器（一般通用）.
    /// </summary>
    public class LogicOrParser : LogicParser
    {
        /// <summary>
        /// 创建一个逻辑或运算表达式的解释器对象实例.
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public LogicOrParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释逻辑或运算表达式.
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            LogicOrDescription logicOr = (LogicOrDescription)this.Description;
            StringBuilder cBuffer = new StringBuilder();
            cBuffer.Append(ParseElementObject(ref DbParameters, logicOr.LeftElement));
            cBuffer.Append(" OR");
            cBuffer.Append(ParseElementObject(ref DbParameters, logicOr.RightElement));
            return cBuffer.ToString();
        }
    }

    /// <summary>
    /// 逻辑非运算表达式的解释器（一般通用）.
    /// </summary>
    public class LogicNotParser : LogicParser
    {
        /// <summary>
        /// 创建一个逻辑非运算表达式的解释器对象实例.
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public LogicNotParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释逻辑非运算表达式.
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            LogicNotDescription logicNot = (LogicNotDescription)this.Description;
            List<Type> allowTypes = new List<Type>(new Type[] {
                typeof(FieldDescription), typeof(ExpDescription), typeof(FunDescription), typeof(GroupDescription)
            });
            IDescription desObject = logicNot.Expression;
            Type expType = desObject.GetType();
            if (!(allowTypes.Contains(expType)))
                throw new NotSupportedException(string.Format("Can not use this description object: {0} for logical not expression.", expType.FullName));
            desObject.DescriptionParserAdapter = this.Adapter;
            string buffer = desObject.GetParser().Parsing(ref DbParameters);
            if (buffer[0] == (char)0x20)
                return string.Format(" NOT{0}", buffer);
            else
                return string.Format(" NOT {0}", buffer);
        }
    }
}
