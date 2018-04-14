using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.SQLite3.CommandParser
{
    /// <summary>
    /// SQLite LIKE 子句解释器.
    /// </summary>
    public class SqliteLikeParser : LikeParser
    {
        /// <summary>
        /// 创建一个 <see cref="SqliteLikeParser"/> 的对象实例.
        /// </summary>
        /// <param name="adapter"></param>
        public SqliteLikeParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释 LIKE 子句。
        /// </summary>
        /// <param name="DbParameters">在解释过程中可能会产生的 DbParameter 参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            LikeDescription ld = (LikeDescription)this.Description;
            StringBuilder cBuffer = new StringBuilder(" ");
            ld.Field.DescriptionParserAdapter = ld.DescriptionParserAdapter;
            cBuffer.AppendFormat("{0} LIKE ", ld.Field.GetParser().Parsing(ref DbParameters));
            switch (ld.Match)
            {
                case LikeMatch.Left:
                    cBuffer.AppendFormat("'{1}{0}'", MatchChar, ld.Content);
                    break;
                case LikeMatch.Right:
                    cBuffer.AppendFormat("'{0}{1}'", MatchChar, ld.Content);
                    break;
                default: // 默认为中间匹配。
                    cBuffer.AppendFormat("'{0}{1}{0}'", MatchChar, ld.Content);
                    break;
            }
            return cBuffer.ToString();
        }
    }
}
