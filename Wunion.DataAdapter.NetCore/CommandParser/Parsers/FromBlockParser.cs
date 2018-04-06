using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// FROM 子句的解释器。
    /// </summary>
    public class FromBlockParser : ParserBase
    {
        /// <summary>
        /// 实例化一个 FROM 子句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public FromBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 FROM 子句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            FromBlock fb = (FromBlock)this.Description;
            StringBuilder cBuffer = new StringBuilder(" FROM");
            foreach (object item in fb.Content)
            {
                if (item is IDescription)
                {
                    IDescription D = (IDescription)item;
                    D.DescriptionParserAdapter = fb.DescriptionParserAdapter;
                    cBuffer.Append(D.GetParser().Parsing(ref DbParameters));
                }
                else
                {
                    if (cBuffer[cBuffer.Length - 1] == (char)0x20)
                        cBuffer.Append(item);
                    else
                        cBuffer.AppendFormat(" {0}", item);
                }
            }
            return cBuffer.ToString();
        }
    }
}
