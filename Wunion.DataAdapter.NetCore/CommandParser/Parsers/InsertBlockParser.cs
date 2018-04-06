using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// INSERT 语句解释器。
    /// </summary>
    public class InsertBlockParser : ParserBase
    {
        /// <summary>
        /// 创建一个 INSERT 语句解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public InsertBlockParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 VALUES 子句中的值元素。
        /// </summary>
        /// <param name="field">与该值对应的字段。</param>
        /// <param name="v">值。</param>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        protected virtual string ParsingValueItem(string field, object v, ref List<IDbDataParameter> DbParameters)
        {
            if (v is IDescription)
            {   // 为表达式时。
                IDescription D = (IDescription)v;
                D.DescriptionParserAdapter = this.Description.DescriptionParserAdapter;
                string buffer = D.GetParser().Parsing(ref DbParameters);
                if (buffer[0] == (char)0x20)
                    buffer = buffer.Remove(0, 1);
                return buffer;
            }
            else
            {   // 为值时。
                IDbDataParameter p = Adapter.CreateDbParameter(string.Format("u_{0}", field), v);
                AddDbParameter(ref DbParameters, p);
                return p.ParameterName;
            }
        }

        /// <summary>
        /// 解释 INSERT 语句。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            InsertBlock InsBlock = (InsertBlock)this.Description;
            StringBuilder cBuffer = new StringBuilder("INSERT INTO");
            StringBuilder valuesBuf = new StringBuilder(") VALUES (");
            InsBlock.Table.DescriptionParserAdapter = InsBlock.DescriptionParserAdapter;
            cBuffer.Append(InsBlock.Table.GetParser().Parsing(ref DbParameters));
            cBuffer.Append(" (");
            InsBlock.Fields[0].DescriptionParserAdapter = InsBlock.DescriptionParserAdapter;
            cBuffer.Append(InsBlock.Fields[0].GetParser().Parsing(ref DbParameters));
            valuesBuf.Append(ParsingValueItem(InsBlock.Fields[0].FieldName, InsBlock.InValues[0], ref DbParameters));
            // 仅利用一次循环来同时解释构建字段信息及其值。
            if (InsBlock.Fields.Count > 1)
            {
                for (int i = 1; i < InsBlock.Fields.Count; ++i)
                {
                    InsBlock.Fields[i].DescriptionParserAdapter = InsBlock.DescriptionParserAdapter;
                    cBuffer.AppendFormat(", {0}", InsBlock.Fields[i].GetParser().Parsing(ref DbParameters));
                    valuesBuf.AppendFormat(", {0}", ParsingValueItem(InsBlock.Fields[i].FieldName, InsBlock.InValues[i], ref DbParameters));
                }
            }
            cBuffer.Append(valuesBuf.ToString());
            cBuffer.Append(")");
            return cBuffer.ToString();
        }
    }
}
