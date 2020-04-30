using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.MySQL.CommandParser
{
    /// <summary>
    /// MySQL LIKE 子句解释器.
    /// </summary>
    public class MySqlLikeParser : LikeParser
    {
        /// <summary>
        /// 创建一个 <see cref="MySqlLikeParser"/> 的对象实例.
        /// </summary>
        /// <param name="adapter"></param>
        public MySqlLikeParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释 LIKE 子句.
        /// </summary>
        /// <param name="DbParameters">在解释过程中可能会产生的 DbParameter 参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            LikeDescription ld = (LikeDescription)this.Description;
            StringBuilder cBuffer = new StringBuilder(" ");
            ld.Field.DescriptionParserAdapter = ld.DescriptionParserAdapter;
            cBuffer.AppendFormat("{0} LIKE ", ld.Field.GetParser().Parsing(ref DbParameters));
            // 参数化组织 LIKE 子句消除SQL注入漏洞。
            IDbDataParameter lp = Adapter.CreateDbParameter("LIKE_KEYWORDS", FormatKeywords(ld.Content));
            AddDbParameter(ref DbParameters, lp);
            switch (ld.Match)
            {
                case LikeMatch.Left:
                    cBuffer.AppendFormat("CONCAT({1},'{0}')", MatchChar, lp.ParameterName);
                    break;
                case LikeMatch.Right:
                    cBuffer.AppendFormat("CONCAT('{0}',{1})", MatchChar, lp.ParameterName);
                    break;
                default: // 默认为中间匹配。
                    cBuffer.AppendFormat("CONCAT('{0}',{1},'{0}')", MatchChar, lp.ParameterName);
                    break;
            }
            return cBuffer.ToString();
        }
    }
}
