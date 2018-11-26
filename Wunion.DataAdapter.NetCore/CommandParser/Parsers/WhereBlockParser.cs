using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// WHERE 子句的解释器。
    /// </summary>
    public class WhereBlockParser : ParserBase
    {
        /// <summary>
        /// 实例化一个 WHERE 子句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public WhereBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 WHERE 子句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            WhereBlock wb = (WhereBlock)this.Description;
            if (wb.Content == null)
                return string.Empty;
            if (wb.Content.Count < 1)
                return string.Empty;
            StringBuilder cBuffer = new StringBuilder(" WHERE");
            IDescription D;
            string item_buf;
            foreach (object item in wb.Content)
            {
                if (item is IDescription)
                {
                    D = (IDescription)item;
                    D.DescriptionParserAdapter = wb.DescriptionParserAdapter;
                    item_buf = D.GetParser().Parsing(ref DbParameters);
                    if (item_buf[0] == (char)0x20) // 检查并补空格（与前一元素间没有空格可能会出错。）
                        cBuffer.Append(item_buf);
                    else
                        cBuffer.AppendFormat(" {0}", item_buf);
                }
                else if (item is char)
                {
                    switch ((char)item)
                    {
                        case '&':
                            cBuffer.Append(" AND");
                            break;
                        case '|':
                            cBuffer.Append(" OR");
                            break;
                        case '(':
                            cBuffer.Append(" (");
                            break;
                        case ')':
                            cBuffer.Append(")");
                            break;
                    }
                }
            }
            return cBuffer.ToString();
        }
    }
}
