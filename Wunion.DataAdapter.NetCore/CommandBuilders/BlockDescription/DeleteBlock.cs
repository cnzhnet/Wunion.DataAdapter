using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述 Delete 命令段的对象类型（多表删除暂不支持）。
    /// </summary>
    public class DeleteBlock : ParseDescription
    {
        private WhereBlock _Conditions;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.DeleteBlock"/> 的对象实例。
        /// </summary>
        public DeleteBlock()
        { }

        /// <summary>
        /// 获取或设置表信息。
        /// </summary>
        public TableDescription Table
        {
            get;
            set;
        }

        /// <summary>
        /// 获取删除命令的 Where  段。
        /// </summary>
        public WhereBlock Conditions
        {
            get { return _Conditions; }
        }

        /// <summary>
        /// 设置命令的 Where 条件信息。
        /// </summary>
        /// <param name="content">条件信息。</param>
        /// <returns></returns>
        public DeleteBlock Where(params object[] content)
        {
            _Conditions = new WhereBlock();
            Conditions.Content.AddRange(content);
            return this;
        }
    }
}
