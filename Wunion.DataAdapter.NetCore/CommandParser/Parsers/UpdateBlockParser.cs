using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// UPDATE 语句解释器。
    /// </summary>
    public class UpdateBlockParser : ParserBase
    {
        /// <summary>
        /// 实例化一个 UPDATE 语句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public UpdateBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 UPDATE 命令。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            UpdateBlock ub = (UpdateBlock)this.Description;
            StringBuilder cBuffer = new StringBuilder("UPDATE");
            string item_buf;
            foreach (IDescription d in ub.Blocks)
            {
                d.DescriptionParserAdapter = ub.DescriptionParserAdapter;
                item_buf = d.GetParser().Parsing(ref DbParameters);
                if (item_buf[0] == (char)0x20)
                    cBuffer.Append(item_buf);
                else
                    cBuffer.AppendFormat(" {0}", item_buf);
            }
            return cBuffer.ToString();
        }
    }
}
