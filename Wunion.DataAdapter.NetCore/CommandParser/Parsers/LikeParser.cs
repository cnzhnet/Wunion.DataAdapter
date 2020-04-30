using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// LIKE 子句解释器。
    /// </summary>
    public class LikeParser : ParserBase
    {
        /// <summary>
        /// 实例化一个LIKE 子句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public LikeParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 获取配置符（不同的数据库类型需子类重写）。
        /// </summary>
        protected virtual char MatchChar
        {
            get { return '%'; }
        }

        /// <summary>
        /// 格式化关键字的内容.
        /// </summary>
        /// <param name="content">原始关键字内容.</param>
        /// <returns></returns>
        protected virtual string FormatKeywords(object content)
        {
            if (content == null || content == DBNull.Value)
                return string.Empty;
            return content.ToString();
        }

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
            // 参数化组织 LIKE 子句消除SQL注入漏洞。
            IDbDataParameter lp = Adapter.CreateDbParameter("LIKE_KEYWORDS", ld.Content);
            AddDbParameter(ref DbParameters, lp);
            switch (ld.Match)
            {
                case LikeMatch.Left:
                    cBuffer.AppendFormat("{1} + '{0}'", MatchChar, lp.ParameterName);
                    break;
                case LikeMatch.Right:
                    cBuffer.AppendFormat("'{0}' + {1}", MatchChar, lp.ParameterName);
                    break;
                default: // 默认为中间匹配。
                    cBuffer.AppendFormat("'{0}' + {1} + '{0}'", MatchChar, lp.ParameterName);
                    break;
            }
            return cBuffer.ToString();
        }
    }
}
