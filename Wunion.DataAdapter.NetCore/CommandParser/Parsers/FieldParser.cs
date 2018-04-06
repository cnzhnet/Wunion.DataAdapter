using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 字段元素的解释器（通用实现，部份数据库可能会出差距题，可参照此方式实现）。
    /// </summary>
    public class FieldParser : ParserBase
    {
        /// <summary>
        /// 实例化一个字段描述信息解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public FieldParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释 Description 对象。
        /// </summary>
        /// <param name="DbParameters">在解释过程中可能会产生的 DbParameter 参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            FieldDescription fdes = (FieldDescription)this.Description;
            string cBuffer;
            if (string.IsNullOrEmpty(fdes.TableName))
                cBuffer = fdes.FieldName == "*" ? "*" : string.Format("{0}{1}{2}", ElemIdentifierL, fdes.FieldName, ElemIdentifierR);
            else
                cBuffer = fdes.FieldName == "*" ? string.Format("{0}.*", fdes.TableName) : string.Format("{0}.{1}{2}{3}", fdes.TableName, ElemIdentifierL, fdes.FieldName, ElemIdentifierR);
            return cBuffer;
        }
    }
}
