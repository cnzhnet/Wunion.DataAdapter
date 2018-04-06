using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述 INSERT 命令的对象类型。
    /// </summary>
    public class InsertBlock : ParseDescription
    {
        private List<object> _Values;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.InsertBlock"/> 的对象实例。
        /// </summary>
        public InsertBlock()
        {
            Fields = new List<FieldDescription>();
            _Values = new List<object>();
        }
        /// <summary>
        /// 获取或设置要插和的表名。
        /// </summary>
        public TableDescription Table
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置 INSERT 段的字段名。
        /// </summary>
        public List<FieldDescription> Fields
        {
            get;
            set;
        }

        /// <summary>
        /// 获取插入的值信息。
        /// </summary>
        public List<object> InValues
        {
            get { return _Values; }
        }

        /// <summary>
        /// 设置插入的值信息。
        /// </summary>
        /// <param name="values"></param>
        public InsertBlock Values(params object[] values)
        {
            _Values.AddRange(values);
            return this;
        }
    }
}
