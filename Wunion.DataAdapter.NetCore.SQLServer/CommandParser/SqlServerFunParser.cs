using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.SQLServer.CommandParser
{
    /// <summary>
    /// Microsoft SQL Server 的函数解释器。
    /// </summary>
    public class SqlServerFunParser : FunParser
    {
        /// <summary>
        /// 实例化一个 Microsoft SQL Server 的函数解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public SqlServerFunParser(ParserAdapter adapter) : base(adapter)
        { }

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
                case "DATEPART":
                    return DatePartParsing(FunD, ref DbParameters);
                default:
                    return base.Parsing(ref DbParameters);
            }
        }

        /// <summary>
        /// Now 函数解释。
        /// </summary>
        /// <param name="D"></param>
        /// <param name="DbParameters"></param>
        /// <returns></returns>
        protected override string NowParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            return "GETDATE()";
        }

        /// <summary>
        /// Len 函数解释。
        /// </summary>
        /// <param name="D"></param>
        /// <param name="DbParameters"></param>
        /// <returns></returns>
        protected override string LenParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            if (D.Parameter is IDescription)
            {
                IDescription desObject = (IDescription)(D.Parameter);
                desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
                string buf = desObject.GetParser().Parsing(ref DbParameters);
                if (buf[0] == (char)0x20)
                    buf = buf.Remove(0, 1);
                return string.Format("LEN({0})", buf);
            }
            else
            {
                IDbDataParameter p = Adapter.CreateDbParameter("uf_len_param", D.Parameter);
                AddDbParameter(ref DbParameters, p);
                return string.Format("LEN({0})", p.ParameterName);
            }
        }

        /// <summary>
        /// Substring 函数解释。
        /// </summary>
        /// <param name="D"></param>
        /// <param name="DbParameters"></param>
        /// <returns></returns>
        protected override string SubstringParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            object[] ps = (object[])D.Parameter;
            if (ps[0] is IDescription)
            {
                IDescription desObject = (IDescription)ps[0];
                desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
                string buf = desObject.GetParser().Parsing(ref DbParameters);
                if (buf[0] == (char)0x20)
                    buf = buf.Remove(0, 1);
                return string.Format("SUBSTRING({0}, {1}, {2})", buf, ps[1], ps[2]);
            }
            else
            {
                IDbDataParameter p = Adapter.CreateDbParameter("uf_substring", ps[0]);
                AddDbParameter(ref DbParameters, p);
                return string.Format("SUBSTRING({0}, {1}, {2})", p.ParameterName, ps[1], ps[2]);
            }
        }

        /// <summary>
        /// DatePart 函数解释。
        /// </summary>
        /// <param name="D"></param>
        /// <param name="DbParameters"></param>
        /// <returns></returns>
        protected string DatePartParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            object[] arry = (object[])D.Parameter;
            StringBuilder fBuffer = new StringBuilder("DATEPART(");
            switch ((DateInterval)arry[0])
            {
                case DateInterval.Year:
                    fBuffer.Append("YYYY, ");
                    break;
                case DateInterval.Quarter:
                    fBuffer.Append("QQ, ");
                    break;
                case DateInterval.Month:
                    fBuffer.Append("MM, ");
                    break;
                case DateInterval.Day:
                    fBuffer.Append("DD, ");
                    break;
                case DateInterval.DayOfYear:
                    fBuffer.Append("DY, ");
                    break;
                case DateInterval.Week:
                    fBuffer.Append("WW, ");
                    break;
                case DateInterval.WeekDay:
                    fBuffer.Append("DW, ");
                    break;
                case DateInterval.Hour:
                    fBuffer.Append("HH, ");
                    break;
                case DateInterval.Minute:
                    fBuffer.Append("MI, ");
                    break;
                case DateInterval.Second:
                    fBuffer.Append("SS, ");
                    break;
                default:
                    fBuffer.Append("MS, ");
                    break;
            }
            if (arry[1] is IDescription)
            {
                IDescription desObject = (IDescription)arry[1];
                desObject.DescriptionParserAdapter = D.DescriptionParserAdapter;
                fBuffer.AppendFormat(((IDescription)arry[1]).GetParser().Parsing(ref DbParameters));
            }
            else
            {
                IDbDataParameter p = Adapter.CreateDbParameter("uf_datepart", arry[1]);
                AddDbParameter(ref DbParameters, p);
                fBuffer.AppendFormat("{0}", p.ParameterName);
            }
            fBuffer.Append(")");
            return fBuffer.ToString();
        }
    }
}
