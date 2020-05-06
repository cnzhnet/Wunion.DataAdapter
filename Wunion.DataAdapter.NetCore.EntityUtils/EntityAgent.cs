using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 表示实体类的查询字段代理辅助类.
    /// </summary>
    public abstract class EntityAgent
    {
        /// <summary>
        /// 创建一个 <see cref="EntityAgent"/> 的对象实例.
        /// </summary>
        public EntityAgent()
        {
            IncludeTableName = false;
        }

        /// <summary>
        /// 获取或设置在生成字段描述时是否包含表名称.
        /// </summary>
        internal bool IncludeTableName { get; set; }

        /// <summary>
        /// 获取或设置代理辅助类所关联的数据表上下文对象.
        /// </summary>
        public TableMapper TableContext { get; set; }

        /// <summary>
        /// 获取字段的描述对象。
        /// </summary>
        /// <param name="name">字段名称.</param>
        /// <returns></returns>
        protected FieldDescription GetField(string name)
        {
            if (TableContext == null)
                throw new Exception(string.Format("{0}.TableContext is null.", this.GetType().FullName));
            if (IncludeTableName)
                return td.Field(TableContext.GetTableName(), name);
            else
                return td.Field(name);
        }

        /// <summary>
        /// 创建该代理类代理的数据表上下文对象.
        /// </summary>
        /// <returns></returns>
        public virtual TableMapper CreateContext()
        { return null; }

        /// <summary>
        /// 获取所有要查询的字段信息的数组.
        /// </summary>
        /// <returns></returns>
        internal IDescription[] GetFieldsArray()
        {
            List<IDescription> fields = new List<IDescription>();
            PropertyInfo[] ps = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (ps != null && ps.Length > 0)
            {
                IDescription buffer = null;
                foreach (PropertyInfo pi in ps)
                {
                    if (pi.PropertyType.IsValueType)
                        continue;
                    buffer = pi.GetValue(this) as IDescription;
                    if (buffer == null)
                        continue;
                    fields.Add(buffer);
                }
            }
            if (fields.Count < 1)
                fields.Add(td.Field("*"));
            return fields.ToArray();
        }
    }
}
