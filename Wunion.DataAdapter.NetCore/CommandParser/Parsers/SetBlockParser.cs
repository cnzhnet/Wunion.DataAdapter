using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// SET 子句解释器。
    /// </summary>
    public class SetBlockParser : ParserBase
    {
        /// <summary>
        /// 实例化一个 SET 子句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public SetBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 SET 子句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            SetBlock sb = (SetBlock)this.Description;
            StringBuilder cBuffer = new StringBuilder(" SET");
            sb.Expressions[0].DescriptionParserAdapter = sb.DescriptionParserAdapter;
            string itemBuffer = sb.Expressions[0].GetParser().Parsing(ref DbParameters);
            if (itemBuffer[0] == (char)0x20)
                cBuffer.Append(itemBuffer);
            else
                cBuffer.AppendFormat(" {0}", itemBuffer);
            if (sb.Expressions.Count > 1)
            {
                for (int i = 1; i < sb.Expressions.Count; ++i)
                {
                    sb.Expressions[i].DescriptionParserAdapter = sb.DescriptionParserAdapter;
                    itemBuffer = sb.Expressions[i].GetParser().Parsing(ref DbParameters);
                    if (itemBuffer[0] == (char)0x20)
                        cBuffer.AppendFormat(",{0}", itemBuffer);
                    else
                        cBuffer.AppendFormat(", {0}", itemBuffer);
                }
            }
            return cBuffer.ToString();
        }
    }
}
