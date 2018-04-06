using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.SQLite3.CommandParser
{
    /// <summary>
    /// SQLite 数据库的 SELECT 命令解释器。
    /// </summary>
    public class SqliteSelectBlockParser : SelectBlockParser
    {
        /// <summary>
        /// 实例化一个 SQLite 数据库的 SELECT 命令解释器。
        /// </summary>
        /// <param name="adapter"></param>
        public SqliteSelectBlockParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 按分页的方式解释 SELECT 命令。
        /// </summary>
        /// <param name="sBlock">SelectBlock对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected override string ParsingWithPage(SelectBlock sBlock, ref List<IDbDataParameter> DbParameters)
        {
            int offset;
            if (sBlock.Pager.CurrentPage > 1)
                offset = sBlock.Pager.PageSize * (sBlock.Pager.CurrentPage - 1);
            else
                offset = 0;
            StringBuilder cBuffer = new StringBuilder(ParsingWithNotPage(sBlock, ref DbParameters));
            // 在结尾处添加 LIMIT ... OFFSET ... 语句实现分页。
            cBuffer.AppendFormat(" LIMIT {0} OFFSET {1}", sBlock.Pager.PageSize, offset);
            return cBuffer.ToString();
        }
    }
}
