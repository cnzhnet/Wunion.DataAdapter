using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 内置函数解析器。
    /// </summary>
    public class FunParser : ParserBase
    {
        public FunParser(ParserAdapter adapter)
            : base(adapter)
        { }

        #region 具体每个函数的解释实现代码，不同的数据库类型由其相应子类重写

        /// <summary>
        /// Distinct 去重复值函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string DistinctParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            FieldDescription[] fields = D.Parameter as FieldDescription[];
            if (fields != null)
            {
                StringBuilder buf = new StringBuilder("DISTINCT ");
                fields[0].DescriptionParserAdapter = D.DescriptionParserAdapter;
                buf.Append(fields[0].GetParser().Parsing(ref DbParameters));
                if (fields.Length > 1)
                {
                    for (int i = 1; i < fields.Length; ++i)
                    {
                        fields[i].DescriptionParserAdapter = D.DescriptionParserAdapter;
                        buf.AppendFormat(", {0}", fields[i].GetParser().Parsing(ref DbParameters));
                    }
                }
                return buf.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Count 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string CountParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is IDescription)
            {   // 参数为表达式或都字段。
                IDescription pDes = (IDescription)(D.Parameter);
                pDes.DescriptionParserAdapter = D.DescriptionParserAdapter;
                string param_buf = pDes.GetParser().Parsing(ref DbParameters);
                if (param_buf[0] == (char)0x20)
                    param_buf = param_buf.Remove(0, 1);
                return string.Format("COUNT({0})", param_buf);
            }
            else if (D.Parameter is string || D.Parameter is char)
            {
                string param_v = D.Parameter.ToString();
                if (param_v == "*")
                {   // COUNT(*) 解释。
                    return "COUNT(*)";
                }
                else
                {
                    throw new Exception("Count 函数使用了不正确的参数。");
                }
            }
            else
            {   // 其它情况。
                IDbDataParameter p = Adapter.CreateDbParameter("uf_count", D.Parameter);
                AddDbParameter(ref DbParameters, p);
                return string.Format("COUNT({0})", p.ParameterName);
            }
        }

        /// <summary>
        /// Sum 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string SumParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is IDescription)
            {   // 参数为表达式或都字段。
                IDescription pDes = (IDescription)(D.Parameter);
                pDes.DescriptionParserAdapter = D.DescriptionParserAdapter;
                string param_buf = pDes.GetParser().Parsing(ref DbParameters);
                if (param_buf[0] == (char)0x20)
                    param_buf = param_buf.Remove(0, 1);
                return string.Format("SUM({0})", param_buf);
            }
            else
            {   // 其它情况。
                IDbDataParameter p = Adapter.CreateDbParameter("uf_sum", D.Parameter);
                AddDbParameter(ref DbParameters, p);
                return string.Format("SUM({0})", p.ParameterName);
            }
        }

        /// <summary>
        /// Avg 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string AvgParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is IDescription)
            {   // 参数为表达式或都字段。
                IDescription pDes = (IDescription)(D.Parameter);
                pDes.DescriptionParserAdapter = D.DescriptionParserAdapter;
                string param_buf = pDes.GetParser().Parsing(ref DbParameters);
                if (param_buf[0] == (char)0x20)
                    param_buf = param_buf.Remove(0, 1);
                return string.Format("AVG({0})", param_buf);
            }
            else
            {   // 其它情况。
                IDbDataParameter p = Adapter.CreateDbParameter("uf_avg", D.Parameter);
                AddDbParameter(ref DbParameters, p);
                return string.Format("AVG({0})", p.ParameterName);
            }
        }

        /// <summary>
        /// Max 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string MaxParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is IDescription)
            {   // 参数为表达式或都字段。
                IDescription pDes = (IDescription)(D.Parameter);
                pDes.DescriptionParserAdapter = D.DescriptionParserAdapter;
                string param_buf = pDes.GetParser().Parsing(ref DbParameters);
                if (param_buf[0] == (char)0x20)
                    param_buf = param_buf.Remove(0, 1);
                return string.Format("MAX({0})", param_buf);
            }
            else
            {   // 其它情况。
                IDbDataParameter p = Adapter.CreateDbParameter("uf_max", D.Parameter);
                AddDbParameter(ref DbParameters, p);
                return string.Format("MAX({0})", p.ParameterName);
            }
        }

        /// <summary>
        /// Min 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string MinParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is IDescription)
            {   // 参数为表达式或都字段。
                IDescription pDes = (IDescription)(D.Parameter);
                pDes.DescriptionParserAdapter = D.DescriptionParserAdapter;
                string param_buf = pDes.GetParser().Parsing(ref DbParameters);
                if (param_buf[0] == (char)0x20)
                    param_buf = param_buf.Remove(0, 1);
                return string.Format("MIN({0})", param_buf);
            }
            else
            {   // 其它情况。
                IDbDataParameter p = Adapter.CreateDbParameter("uf_min", D.Parameter);
                AddDbParameter(ref DbParameters, p);
                return string.Format("MIN({0})", p.ParameterName);
            }
        }

        /// <summary>
        /// Now 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string NowParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            throw new NotSupportedException("使用了未被实现的函数：Now");
        }

        /// <summary>
        /// Len 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string LenParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            throw new NotSupportedException("使用了未被实现的函数：Len");
        }

        protected virtual string SubstringParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            throw new NotSupportedException("使用了未被实现的函数：Substring");
        }

        /// <summary>
        /// In 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string InParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            object[] values = (object[])D.Parameter;
            StringBuilder cBuffer = new StringBuilder();
            FieldDescription fd = (FieldDescription)values[0];
            fd.DescriptionParserAdapter = D.DescriptionParserAdapter;
            cBuffer.Append(fd.GetParser().Parsing(ref DbParameters));
            cBuffer.Append(" IN (");
            bool AppendComma = false; // 是否加逗号标识。
            for (int i = 1; i < values.Length; ++i)
            {
                string buf;
                if (values[i] is IDescription)
                {
                    IDescription desObject = (IDescription)values[i];
                    desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
                    buf = desObject.GetParser().Parsing(ref DbParameters);
                    if (buf[0] == (char)0x20)
                        buf = buf.Remove(0, 1);
                }
                else
                {
                    IDbDataParameter p = Adapter.CreateDbParameter(string.Format("uf_inValue{0}", i), values[i]);
                    AddDbParameter(ref DbParameters, p);
                    buf = p.ParameterName;
                }
                if (AppendComma)
                {
                    cBuffer.AppendFormat(", {0}", buf);
                }
                else
                {
                    cBuffer.Append(buf);
                    AppendComma = true;
                }
            }
            cBuffer.Append(")");
            return cBuffer.ToString();
        }

        /// <summary>
        /// NotIn 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string NotInParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            object[] values = (object[])D.Parameter;
            StringBuilder cBuffer = new StringBuilder();
            FieldDescription fd = (FieldDescription)values[0];
            fd.DescriptionParserAdapter = D.DescriptionParserAdapter;
            cBuffer.Append(fd.GetParser().Parsing(ref DbParameters));
            cBuffer.Append(" NOT IN (");
            bool AppendComma = false; // 是否加逗号标识。
            for (int i = 1; i < values.Length; ++i)
            {
                string buf;
                if (values[i] is IDescription)
                {
                    IDescription desObject = (IDescription)values[i];
                    desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
                    buf = desObject.GetParser().Parsing(ref DbParameters);
                    if (buf[0] == (char)0x20)
                        buf = buf.Remove(0, 1);
                }
                else
                {
                    IDbDataParameter p = Adapter.CreateDbParameter(string.Format("uf_inValue{0}", i), values[i]);
                    AddDbParameter(ref DbParameters, p);
                    buf = p.ParameterName;
                }
                if (AppendComma)
                {
                    cBuffer.AppendFormat(", {0}", buf);
                }
                else
                {
                    cBuffer.Append(buf);
                    AppendComma = true;
                }
            }
            cBuffer.Append(")");
            return cBuffer.ToString();
        }

        /// <summary>
        /// IsNull 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string IsNullParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is FieldDescription)
            {
                FieldDescription desObject = (FieldDescription)(D.Parameter);
                desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
                StringBuilder cBuffer = new StringBuilder(desObject.GetParser().Parsing(ref DbParameters));
                cBuffer.Append(" IS NULL");
                return cBuffer.ToString();
            }
            else
            {
                throw new Exception("IsNull 函数的参数必须是一个字段类型。");
            }
        }

        /// <summary>
        /// IsNotNull 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string IsNotNullParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is FieldDescription)
            {
                FieldDescription desObject = (FieldDescription)(D.Parameter);
                desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
                StringBuilder cBuffer = new StringBuilder(desObject.GetParser().Parsing(ref DbParameters));
                cBuffer.Append(" IS NOT NULL");
                return cBuffer.ToString();
            }
            else
            {
                throw new Exception("IsNotNull 函数的参数必须是一个字段类型。");
            }
        }

        /// <summary>
        /// BetweenAnd 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string BetweenAndParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            object[] values = (object[])D.Parameter;
            IDescription desObject = (IDescription)values[0];
            desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
            string buf = desObject.GetParser().Parsing(ref DbParameters);
            if (buf[0] == (char)0x20)
                buf = buf.Remove(0, 1);
            StringBuilder cBuffer = new StringBuilder(buf);
            IDbDataParameter p1 = Adapter.CreateDbParameter("uf_start", values[1]);
            AddDbParameter(ref DbParameters, p1);
            cBuffer.AppendFormat(" BETWEEN {0}", p1.ParameterName);
            IDbDataParameter p2 = Adapter.CreateDbParameter("uf_end", values[2]);
            AddDbParameter(ref DbParameters, p2);
            cBuffer.AppendFormat(" AND {0}", p2.ParameterName);
            return cBuffer.ToString();
        }

        /// <summary>
        /// NotBetweenAnd 函数解释。
        /// </summary>
        /// <param name="D">FunDescription 对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string NotBetweenAndParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            object[] values = (object[])D.Parameter;
            IDescription desObject = (IDescription)values[0];
            desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
            string buf = desObject.GetParser().Parsing(ref DbParameters);
            if (buf[0] == (char)0x20)
                buf = buf.Remove(0, 1);
            StringBuilder cBuffer = new StringBuilder(buf);
            IDbDataParameter p1 = Adapter.CreateDbParameter("uf_start", values[1]);
            AddDbParameter(ref DbParameters, p1);
            cBuffer.AppendFormat(" NOT BETWEEN {0}", p1.ParameterName);
            IDbDataParameter p2 = Adapter.CreateDbParameter("uf_end", values[2]);
            AddDbParameter(ref DbParameters, p2);
            cBuffer.AppendFormat(" AND {0}", p2.ParameterName);
            return cBuffer.ToString();
        }

        #endregion

        /// <summary>
        /// 解释当前的函数语句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            FunDescription FunD = (FunDescription)this.Description;
            switch (FunD.FunctionName)
            {
                case "DISTINCT":
                    return DistinctParsing(FunD, ref DbParameters);
                case "COUNT":
                    return CountParsing(FunD, ref DbParameters);
                case "SUM":
                    return SumParsing(FunD, ref DbParameters);
                case "AVG":
                    return AvgParsing(FunD, ref DbParameters);
                case "MAX":
                    return MaxParsing(FunD, ref DbParameters);
                case "MIN":
                    return MinParsing(FunD, ref DbParameters);
                case "NOW":
                    return NowParsing(FunD, ref DbParameters);
                case "LEN":
                    return LenParsing(FunD, ref DbParameters);
                case "SUBSTRING":
                    return SubstringParsing(FunD, ref DbParameters);
                case "IN":
                    return InParsing(FunD, ref DbParameters);
                case "NOTIN":
                    return NotInParsing(FunD, ref DbParameters);
                case "ISNULL":
                    return IsNullParsing(FunD, ref DbParameters);
                case "ISNOTNULL":
                    return IsNotNullParsing(FunD, ref DbParameters);
                case "BETWEENAND":
                    return BetweenAndParsing(FunD, ref DbParameters);
                case "NOTBETWEENAND":
                    return NotBetweenAndParsing(FunD, ref DbParameters);
                default:
                    throw new NotSupportedException(string.Format("使用了未受支持的函数：{0}", FunD.FunctionName));
            }
        }
    }
}
