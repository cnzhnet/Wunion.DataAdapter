using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// LEFT JOIN 子句解释器。
    /// </summary>
    public class LeftJoinParser : ParserBase
    {
        /// <summary>
        /// 实例化一个 LEFT JOIN 子句解释器。
        /// </summary>
        /// <param name="adapter">该解释器所属的适配器。</param>
        public LeftJoinParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 返回连接多个 ON 条件的与运算.
        /// </summary>
        protected virtual string KeywordsAnd
        {
            get { return "AND"; }
        }

        /// <summary>
        /// 解释 LEFT JOIN 子句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            LeftJoinDescription LeftJoin = (LeftJoinDescription)this.Description;
            StringBuilder cBuffer = new StringBuilder(" LEFT JOIN");
            LeftJoin.Table.DescriptionParserAdapter = LeftJoin.DescriptionParserAdapter;
            cBuffer.Append(LeftJoin.Table.GetParser().Parsing(ref DbParameters));
            cBuffer.Append(" ON");
            ExpDescription ExpDes = (ExpDescription)LeftJoin.OnDescription[0];
            ExpDes.DescriptionParserAdapter = LeftJoin.DescriptionParserAdapter;
            cBuffer.Append(ExpDes.GetParser().Parsing(ref DbParameters));
            if (LeftJoin.OnDescription.Count > 1)
            {
                for (int i = 1; i < LeftJoin.OnDescription.Count; ++i)
                {
                    ExpDes = (ExpDescription)LeftJoin.OnDescription[i];
                    ExpDes.DescriptionParserAdapter = LeftJoin.DescriptionParserAdapter;
                    cBuffer.AppendFormat(" {0} {1}", KeywordsAnd, ExpDes.GetParser().Parsing(ref DbParameters));
                }
            }
            return cBuffer.ToString();
        }
    }
}
