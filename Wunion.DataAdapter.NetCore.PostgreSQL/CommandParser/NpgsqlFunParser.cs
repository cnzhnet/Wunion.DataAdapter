using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser
{
    /// <summary>
    /// PostgreSQL 函数解释器.
    /// </summary>
    public class NpgsqlFunParser : FunParser
    {
        /// <summary>
        /// 创建一个 PostagreSQL 函数解释器.
        /// </summary>
        /// <param name="adapter">所属的父级适配器</param>
        public NpgsqlFunParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// Now 函数解释。
        /// </summary>
        /// <param name="D"></param>
        /// <param name="DbParameters"></param>
        /// <returns></returns>
        protected override string NowParsing(FunDescription D, ref List<IDbDataParameter> DbParameters)
        {
            return "now()";
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
                return string.Format("CHAR_LENGTH({0})", buf);
            }
            else
            {
                IDbDataParameter p = Adapter.CreateDbParameter("uf_len_param", D.Parameter);
                AddDbParameter(ref DbParameters, p);
                return string.Format("CHAR_LENGTH({0})", p.ParameterName);
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
                return string.Format("SUBSTRING({0} FROM {1} FOR {2})", buf, ps[1], ps[2]);
            }
            else
            {
                IDbDataParameter p = Adapter.CreateDbParameter("uf_substring", ps[0]);
                AddDbParameter(ref DbParameters, p);
                return string.Format("SUBSTRING({0} FROM {1} FOR {2})", p.ParameterName, ps[1], ps[2]);
            }
        }
    }
}
