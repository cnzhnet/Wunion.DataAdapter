using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 表元素的解释器。
    /// </summary>
    public class TableParser : ParserBase
    {
        /// <summary>
        /// 创建一个表元素解释器的实例。
        /// </summary>
        /// <param name="adapter">该解释器所属的适配器。</param>
        public TableParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 表元素解释器。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            TableDescription tabDes = (TableDescription)this.Description;
            StringBuilder cBuffer = new StringBuilder(" "); // 空格统一在前面。
            cBuffer.AppendFormat("{0}{1}{2}", ElemIdentifierL, tabDes.Name, ElemIdentifierR);
            if (!string.IsNullOrEmpty(tabDes.Aliases))
                cBuffer.AppendFormat(" {0}", tabDes.Aliases);
            return cBuffer.ToString();
        }
    }
}
