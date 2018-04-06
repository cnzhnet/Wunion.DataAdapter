using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// DELETE 语句解释器。
    /// </summary>
    public class DeleteBlockParser : ParserBase
    {
        /// <summary>
        /// 实例化一个DELETE 语句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public DeleteBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 DELETE 命令。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            DeleteBlock DelBlock = (DeleteBlock)this.Description;
            StringBuilder cBuffer = new StringBuilder("DELETE FROM");

            // 将解析适配器继续向下传递。
            DelBlock.Table.DescriptionParserAdapter = DelBlock.DescriptionParserAdapter;
            DelBlock.Conditions.DescriptionParserAdapter = DelBlock.DescriptionParserAdapter;

            cBuffer.Append(DelBlock.Table.GetParser().Parsing(ref DbParameters));
            cBuffer.Append(DelBlock.Conditions.GetParser().Parsing(ref DbParameters));
            return cBuffer.ToString();
        }
    }
}
