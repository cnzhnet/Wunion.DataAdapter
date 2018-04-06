using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 元素优先级分组（即命令中的括号分组）的解释器.
    /// </summary>
    public class GroupElementParser : ParserBase
    {
        /// <summary>
        /// 创建一个元素优先级分组（即命令中的括号分组）的解释器对象实例.
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public GroupElementParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释元素优先级分组.
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            GroupDescription groupDes = (GroupDescription)this.Description;
            IDescription desObject = groupDes.Content;
            desObject.DescriptionParserAdapter = this.Adapter;
            string buffer = desObject.GetParser().Parsing(ref DbParameters);
            if (buffer[0] == (char)0x20)
                return string.Format(" ({0})", buffer.Remove(0, 1));
            else
                return string.Format(" ({0})", buffer);
        }
    }
}
