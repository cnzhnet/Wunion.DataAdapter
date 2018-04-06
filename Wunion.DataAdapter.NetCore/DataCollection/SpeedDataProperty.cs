using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.DataCollection
{
    /// <summary>
    /// 提供 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/> 类上的属性的抽象化实现。
    /// </summary>
    public class SpeedDataProperty : System.ComponentModel.PropertyDescriptor
    {
        private SpeedDataColumn mDataColumn;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataProperty"/> 的对象实例。
        /// </summary>
        /// <param name="column">属性的名称。</param>
        /// <param name="attributes">包含属性特性的类型 System.Attribute 的数组。</param>
        public SpeedDataProperty(SpeedDataColumn column, Attribute[] attributes) : base(column.Name, attributes)
        {
            mDataColumn = column;
        }

        /// <summary>
        /// 获取该属性绑定到的组件的类型。
        /// </summary>
        public override Type ComponentType
        {
            get { return typeof(SpeedDataRow); }
        }

        /// <summary>
        /// 获取该属性是否为只读。
        /// </summary>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 获取该属性的值类型。
        /// </summary>
        public override Type PropertyType
        {
            get { return mDataColumn.DataType; }
        }

        /// <summary>
        /// 获取是否允许重置值。
        /// </summary>
        /// <param name="component">该属性绑定到的组件对象（为 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/>对象）。</param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// 获取该属性的值。
        /// </summary>
        /// <param name="component">该属性绑定到的组件对象（为 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/>对象）。</param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            return ((SpeedDataRow)component)[mDataColumn.Index];
        }

        /// <summary>
        /// 重置该属性的值。
        /// </summary>
        /// <param name="component">该属性绑定到的组件对象（为 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/>对象）。</param>
        public override void ResetValue(object component)
        {
            ((SpeedDataRow)component)[mDataColumn.Index] = Activator.CreateInstance(mDataColumn.DataType);
        }

        /// <summary>
        /// 设置该属性的值。
        /// </summary>
        /// <param name="component">该属性绑定到的组件对象（为 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/>对象）。</param>
        /// <param name="value">值。</param>
        public override void SetValue(object component, object value)
        {
            ((SpeedDataRow)component)[mDataColumn.Index] = value;
        }

        /// <summary>
        /// 返回是否需要永久保存该属性的值。
        /// </summary>
        /// <param name="component">该属性绑定到的组件对象（为 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/>对象）。</param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        /// <summary>
        /// 获取该成员是否可浏览。
        /// </summary>
        public override bool IsBrowsable
        {
            get { return true; }
        }
    }
}
