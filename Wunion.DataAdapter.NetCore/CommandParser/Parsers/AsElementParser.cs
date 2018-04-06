using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// As别名元素解释器。
    /// </summary>
    public class AsElementParser : ParserBase
    {
        /// <summary>
        /// 实例化一个字段描述信息解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public AsElementParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释整个表达式信息。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            AsElementDecsription elem = (AsElementDecsription)this.Description;
            if (string.IsNullOrEmpty(elem.AsName))
                throw new Exception("未指定 AS 别名，将无法正确解释 AsElementDecsription 。");
            if (elem.Objective is IDescription)
            {
                IDescription Des = (IDescription)elem.Objective;
                Des.DescriptionParserAdapter = elem.DescriptionParserAdapter; // 将解析适配器继续向下传递。
                string buf = Des.GetParser().Parsing(ref DbParameters);
                return string.Format("{0} AS {1}{2}{3}", buf, ElemIdentifierL, elem.AsName, ElemIdentifierR);
            }
            else
            {
                IDbDataParameter p = Adapter.CreateDbParameter("u_ASVAL", elem.Objective);
                AddDbParameter(ref DbParameters, p);
                return string.Format("{0} AS {1}{2}{3}", p.ParameterName, ElemIdentifierL, elem.AsName, ElemIdentifierR);
            }
        }
    }
}
