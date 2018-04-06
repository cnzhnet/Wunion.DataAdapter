using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// SELECT 语句解释器。
    /// </summary>
    public class SelectBlockParser : ParserBase
    {
        /// <summary>
        /// 实例化一个 SELECT 语句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public SelectBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 按不分页的方式解释 SELECT 命令。
        /// </summary>
        /// <param name="sBlock">SelectBlock对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string ParsingWithNotPage(SelectBlock sBlock, ref List<IDbDataParameter> DbParameters)
        {
            StringBuilder cBuffer = new StringBuilder("SELECT ");
            bool AppendComma = true; // 用于标识每个段后是否添加逗号。
            sBlock.Blocks[0].DescriptionParserAdapter = sBlock.DescriptionParserAdapter;
            string item_buf = sBlock.Blocks[0].GetParser().Parsing(ref DbParameters);
            if (item_buf[0] == (char)0x20)
                item_buf = item_buf.Remove(0, 1);
            cBuffer.Append(item_buf);
            for (int i = 1; i < sBlock.Blocks.Count; ++i)
            {
                if (sBlock.Blocks[i] is FromBlock || sBlock.Blocks[i] is WhereBlock || sBlock.Blocks[i] is GroupByBlock || sBlock.Blocks[i] is OrderByBlock)
                    AppendComma = false;
                sBlock.Blocks[i].DescriptionParserAdapter = sBlock.DescriptionParserAdapter;
                item_buf = sBlock.Blocks[i].GetParser().Parsing(ref DbParameters);
                if (item_buf[0] == (char)0x20)
                    cBuffer.AppendFormat("{0}{1}", AppendComma ? "," : string.Empty, item_buf);
                else
                    cBuffer.AppendFormat("{0} {1}", AppendComma ? "," : string.Empty, item_buf);
            }
            return cBuffer.ToString();
        }

        /// <summary>
        /// 按分页的方式解释 SELECT 命令。
        /// </summary>
        /// <param name="sBlock">SelectBlock对象。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string ParsingWithPage(SelectBlock sBlock, ref List<IDbDataParameter> DbParameters)
        {
            return "尚未实现。";
        }

        /// <summary>
        /// 解释 SELECT 命令。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            SelectBlock sBlock = (SelectBlock)this.Description;
            if (sBlock.Pager == null)
                return ParsingWithNotPage(sBlock, ref DbParameters);
            else
                return ParsingWithPage(sBlock, ref DbParameters);
        }
    }
}
