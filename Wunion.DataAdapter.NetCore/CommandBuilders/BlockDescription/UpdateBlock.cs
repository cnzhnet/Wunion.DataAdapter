using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描术 UPDATE 命令的对象类型。
    /// </summary>
    public class UpdateBlock : ParseDescription
    {
        private List<IDescription> _Blocks;

        public UpdateBlock()
        {
            _Blocks = new List<IDescription>();
        }

        /// <summary>
        /// 获取命令中的分段列表。
        /// </summary>
        public List<IDescription> Blocks
        {
            get { return _Blocks; }
        }

        /// <summary>
        /// 向命令中添加一个表。
        /// </summary>
        /// <param name="table"></param>
        public void AddTable(TableDescription table)
        {
            _Blocks.Add(table);
        }

        /// <summary>
        /// 设置字符的更新信息。
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public UpdateBlock Set(params IDescription[] expressions)
        {
            SetBlock setB = new SetBlock();
            setB.Expressions.AddRange(expressions);
            _Blocks.Add(setB);
            return this;
        }

        /// <summary>
        /// 设置更新的条件信息。
        /// </summary>
        /// <param name="content"></param>
        public void Where(params object[] content)
        {
            WhereBlock wb = new WhereBlock();
            wb.Content.AddRange(content);
            _Blocks.Add(wb);
        }
    }
}
