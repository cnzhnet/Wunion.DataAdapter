using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// ORDER BY 子句解释器。
    /// </summary>
    public class OrderByBlockParser : ParserBase
    {
        /// <summary>
        /// 创建一个 ORDER BY 子句解释器实例。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public OrderByBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 ORDER BY 子句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            OrderByBlock orderBy = (OrderByBlock)this.Description;
            StringBuilder cBuffer = new StringBuilder(" ORDER BY");
            orderBy.Field.DescriptionParserAdapter = orderBy.DescriptionParserAdapter;
            cBuffer.AppendFormat(" {0}", orderBy.Field.GetParser().Parsing(ref DbParameters));
            switch (orderBy.Sort)
            {
                case OrderByMode.ASC:
                    cBuffer.AppendFormat(" ASC");
                    break;
                default:
                    cBuffer.AppendFormat(" DESC");
                    break;
            }
            return cBuffer.ToString();
        }
    }
}
