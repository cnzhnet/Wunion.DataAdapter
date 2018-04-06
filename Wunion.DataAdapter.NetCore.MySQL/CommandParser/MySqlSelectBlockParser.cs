using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.MySQL.CommandParser
{
    /// <summary>
    /// MySQL 数据库的 SELECT 命令解释器。
    /// </summary>
    public class MySqlSelectBlockParser : SelectBlockParser
    {
        /// <summary>
        /// 实例化一个 MySQL 数据库的 SELECT 命令解释器。
        /// </summary>
        /// <param name="adapter"></param>
        public MySqlSelectBlockParser(ParserAdapter adapter)
            : base(adapter)
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
            cBuffer.AppendFormat(" LIMIT {0},{1}", offset, sBlock.Pager.PageSize * sBlock.Pager.CurrentPage);
            return cBuffer.ToString();
        }
    }
}
