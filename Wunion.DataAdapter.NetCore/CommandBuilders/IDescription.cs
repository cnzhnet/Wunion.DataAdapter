using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 表示命令或命令元素的描述对象基础类型接口。
    /// </summary>
    public interface IDescription
    {
        /// <summary>
        /// 获取或设置解释该对象的适配器。
        /// </summary>
        ParserAdapter DescriptionParserAdapter
        {
            get;
            set;
        }

        /// <summary>
        /// 获取用于解释该对象的解析器。
        /// </summary>
        /// <returns></returns>
        ParserBase GetParser();
    }
}
