using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.SQLServer.CommandParser
{
    /// <summary>
    /// Microsoft SQL Server 2005 及以上版本的 SELECT 命令解释器。
    /// </summary>
    public class SqlServerSelectBlockParser : SelectBlockParser
    {
        /// <summary>
        /// 实例化一个 Microsoft SQL Server 2005 及以上版本的 SELECT 命令解释器
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public SqlServerSelectBlockParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 分布关键的命令部份解释。
        /// </summary>
        /// <param name="sBlock">SELECT命令段描述对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        private string ParsingCore(SelectBlock sBlock, ref List<IDbDataParameter> DbParameters)
        {
            bool AppendComma = true; // 用于标识每个段后是否添加逗号。
            sBlock.Blocks[0].DescriptionParserAdapter = sBlock.DescriptionParserAdapter;
            string item_buf = sBlock.Blocks[0].GetParser().Parsing(ref DbParameters);
            if (item_buf[0] == (char)0x20)
                item_buf = item_buf.Remove(0, 1);
            StringBuilder FieldsBuffer = new StringBuilder(item_buf); // 查询的字段信息部份
            StringBuilder OtherBuffer = new StringBuilder(); // 除字段信息之外的其它子句部份
            OrderByBlock OverOrderBy = null;
            for (int i = 1; i < sBlock.Blocks.Count; ++i)
            {
                if (sBlock.Blocks[i] is FromBlock || sBlock.Blocks[i] is WhereBlock || sBlock.Blocks[i] is GroupByBlock)
                    AppendComma = false;
                if (sBlock.Blocks[i] is OrderByBlock)
                {
                    AppendComma = false;
                    OverOrderBy = (OrderByBlock)sBlock.Blocks[i];
                    continue; // 不在内嵌SELECT子句中生成 ORDER BY 子句（否则会产生查询错误）
                }
                sBlock.Blocks[i].DescriptionParserAdapter = sBlock.DescriptionParserAdapter;
                item_buf = sBlock.Blocks[i].GetParser().Parsing(ref DbParameters);
                if (AppendComma)
                {
                    if (item_buf[0] == (char)0x20)
                        FieldsBuffer.AppendFormat(",{0}", item_buf);
                    else
                        FieldsBuffer.AppendFormat(", {0}", item_buf);
                }
                else
                {
                    if (item_buf[0] == (char)0x20)
                        OtherBuffer.Append(item_buf);
                    else
                        OtherBuffer.AppendFormat(" {0}", item_buf);
                }
            }
            if (OverOrderBy == null)
            {
                if (IsNull(sBlock.Pager.HelpField))
                    throw (new Exception("解释分页时失败：没有提供排序字段。"));
                OverOrderBy = new OrderByBlock();
                OverOrderBy.Field = sBlock.Pager.HelpField;
                OverOrderBy.Sort = sBlock.Pager.HelpSort;
            }
            StringBuilder SqlCore = new StringBuilder("WITH [PAGE_TEMP] AS (SELECT ");
            OverOrderBy.DescriptionParserAdapter = sBlock.DescriptionParserAdapter;
            SqlCore.AppendFormat("{0}, ROW_NUMBER() OVER({1}) AS [ROWNUM]", FieldsBuffer, OverOrderBy.GetParser().Parsing(ref DbParameters));
            SqlCore.Append(OtherBuffer);
            SqlCore.AppendFormat(") ");
            return SqlCore.ToString();
        }

        /// <summary>
        /// 按分页的方式解释 SELECT 命令。
        /// </summary>
        /// <param name="sBlock">SelectBlock对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected override string ParsingWithPage(SelectBlock sBlock, ref List<IDbDataParameter> DbParameters)
        {
            StringBuilder CommandText = new StringBuilder(ParsingCore(sBlock, ref DbParameters));
            if (sBlock.Pager.CurrentPage > 1)
            {
                int Offset = (sBlock.Pager.CurrentPage - 1) * sBlock.Pager.PageSize + 1;
                int Limit = sBlock.Pager.CurrentPage * sBlock.Pager.PageSize;
                CommandText.AppendFormat("SELECT * FROM [PAGE_TEMP] WHERE [ROWNUM] BETWEEN {0} AND {1}", Offset, Limit);
            }
            else
            {
                CommandText.AppendFormat("SELECT TOP {0} * FROM [PAGE_TEMP]", sBlock.Pager.PageSize);
            }
            return CommandText.ToString();
        }
    }
}
