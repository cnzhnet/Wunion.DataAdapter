using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 可解释描述对象的基础类型。
    /// </summary>
    public abstract class ParseDescription : IDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.ParseDescription"/> 的对象实例.
        /// </summary>
        protected ParseDescription() { }

        /// <summary>
        /// 获取该对象的解释适配器。
        /// </summary>
        public ParserAdapter DescriptionParserAdapter
        {
            get;
            set;
        }

        /// <summary>
        /// 获取与该对象相关的解释器。
        /// </summary>
        /// <returns></returns>
        public ParserBase GetParser()
        {
            if (DescriptionParserAdapter == null)
                throw (new Exception("ParserAdapter is unknown."));
            return DescriptionParserAdapter.GetParserByObject(this);
        }
    }
}
