using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// GROUP BY 子句解释器。
    /// </summary>
    public class GroupByBlockParser : ParserBase
    {
        /// <summary>
        /// 实例化一个 GROUP BY 子解释器
        /// </summary>
        /// <param name="adapter"></param>
        public GroupByBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 GROUP BY 子句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            GroupByBlock grpB = (GroupByBlock)this.Description;
            StringBuilder cBuffer = new StringBuilder(" GROUP BY");
            FieldDescription fd = grpB.Fields[0];
            fd.DescriptionParserAdapter = grpB.DescriptionParserAdapter;
            cBuffer.AppendFormat(" {0}", fd.GetParser().Parsing(ref DbParameters));
            if (grpB.Fields.Count > 1)
            {
                for (int i = 1; i < grpB.Fields.Count; ++i)
                {
                    fd = grpB.Fields[i];
                    fd.DescriptionParserAdapter = grpB.DescriptionParserAdapter;
                    cBuffer.AppendFormat(", {0}", fd.GetParser().Parsing(ref DbParameters));
                }
            }
            return cBuffer.ToString();
        }
    }
}
