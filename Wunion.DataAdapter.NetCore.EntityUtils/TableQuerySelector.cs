using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 表示表查询筛选器.
    /// </summary>
    public class TableQuerySelector
    {
        private DbCommandBuilder _dbCommand;
        private SelectBlock selectBlock;
        private string tableName;

        /// <summary>
        /// 创建一个 <see cref="TableQuerySelector"/> 的对象实例.
        /// <param name="tbl_name">针对该表进行查询.</param>
        /// </summary>
        internal TableQuerySelector(string tbl_name)
        {
            _dbCommand = new DbCommandBuilder();
            tableName = tbl_name;
        }

        /// <summary>
        /// 设置要查询的结果子表达式，并返回 SELECT 表达式树对象.
        /// </summary>
        /// <param name="Expressions">要查询的结果表太式（字段信息或函数表达式）</param>
        /// <returns></returns>
        public SelectBlock Select(params object[] Expressions)
        {
            selectBlock = _dbCommand.Select();
            IDescription descr;
            foreach (object desObject in Expressions)
            {
                descr = desObject as IDescription;
                if (descr != null) // 当返回的字段元素非 IDescription 对象时忽略它（否则命令解无法解释）.
                    selectBlock.AddElement(descr);
            }
            selectBlock.From(tableName);
            return selectBlock;
        }

        /// <summary>
        /// 获取查询命令构建器.
        /// </summary>
        internal DbCommandBuilder DbCommand => _dbCommand;
    }
}
