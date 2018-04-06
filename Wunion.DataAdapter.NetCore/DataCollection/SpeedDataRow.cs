using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace Wunion.DataAdapter.Kernel.DataCollection
{
    /// <summary>
    /// 表示数据行的对象。
    /// </summary>
    [Serializable]
    public class SpeedDataRow : System.ComponentModel.ICustomTypeDescriptor, INotifyPropertyChanged
    {
        /// <summary>
        /// 表示数据单元格。
        /// </summary>
        [Serializable]
        protected class DataCell
        {
            /// <summary>
            /// 获取或设置该数据单元格所属的列。
            /// </summary>
            public SpeedDataColumn Column
            {
                get;
                set;
            }

            /// <summary>
            /// 获取或设置该单元格的值。
            /// </summary>
            public object Value
            {
                get;
                set;
            }

            /// <summary>
            /// 创建一个数据单元格。
            /// </summary>
            public DataCell()
            { }

            /// <summary>
            /// 创建一个数据单元格。
            /// </summary>
            /// <param name="OwnerColumn">所属的列。</param>
            /// <param name="Val">值。</param>
            public DataCell(SpeedDataColumn OwnerColumn, object Val)
            {
                Column = OwnerColumn;
                Value = Val;
            }
        }

        /// <summary>
        /// 表示数据字段单元格的集合。
        /// </summary>
        private List<DataCell> DataCells;

        /// <summary>
        /// 表示字段的名称寻址映射 Hash 表（提高通过字段名读取或设置值时的寻址效率）。
        /// </summary>
        private System.Collections.Hashtable CellAddressHashTable;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/> 对象的实例。
        /// </summary>
        public SpeedDataRow()
        {
            DataCells = new List<DataCell>();
            CellAddressHashTable = new System.Collections.Hashtable();
        }

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataRow"/> 对象的实例。
        /// </summary>
        /// <param name="table">该行所属的 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataTable"/> 表对象。</param>
        public SpeedDataRow(SpeedDataTable table)
        {
            DataCells = new List<DataCell>();
            CellAddressHashTable = new System.Collections.Hashtable();
            Table = table;

            // 以下代码为防止错误。
            foreach (SpeedDataColumn Column in Table.Columns)
                Add(Column, Column.DefaultValue);
        }

        /// <summary>
        /// 获取或设置该行所属的 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataTable"/> 表对象。
        /// </summary>
        public SpeedDataTable Table
        {
            get;
            set;
        }

        /// <summary>
        /// 获了或设置指定列索引处的值。
        /// </summary>
        /// <param name="index">列索引。</param>
        /// <returns></returns>
        public object this[int index]
        {
            get { return DataCells[index].Value; }
            set
            {
                DataCells[index].Value = value;
                OnPropertyChanged(DataCells[index].Column.Name);
            }
        }

        /// <summary>
        /// 获取该行上指定列的数据。
        /// </summary>
        /// <param name="ColumnName">列名称。</param>
        /// <returns></returns>
        public object this[string ColumnName]
        {
            get
            {
                DataCell Cell = FindCell(ColumnName);
                return Cell.Value;
            }
            set
            {
                DataCell Cell = FindCell(ColumnName);
                if (Cell == null)
                {
                    SpeedDataColumn Column = Table.Columns[ColumnName];
                    Cell = Add(Column, value);
                    OnPropertyChanged(ColumnName);
                }
                else
                {
                    if (Cell.Column.DataType.IsValueType && value == null)
                    {
                        Cell.Value = Activator.CreateInstance(Cell.Column.DataType);
                    }
                    else
                    {
                        Cell.Value = value;
                    }
                    OnPropertyChanged(ColumnName);
                }
            }
        }

        /// <summary>
        /// 查找指定列所属的数据单元格对象。
        /// </summary>
        /// <param name="ColumnName">列名称。</param>
        /// <returns></returns>
        protected DataCell FindCell(string ColumnName)
        {
            /* 此处不用 CellAddressHashTable.ContainsKey(ColumnName) 做判断，否则会搜索两次 Hash 表。
               在字段存在的情况下 try....catch 并不影响程序性能，返而可以减少一次 Hash 表的搜索 */
            try
            {
                return (DataCell)(CellAddressHashTable[ColumnName]);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 为新列创建一个数据单元格（此方法不可让外部程序集使用）。
        /// </summary>
        /// <param name="Column">数据新单元格所属的列。</param>
        internal void CreateNewCell(SpeedDataColumn Column)
        {
            DataCell Cell = new DataCell(Column, Column.DefaultValue);
            CellAddressHashTable.Add(Column.Name, Cell);
            if (Column.Index >= DataCells.Count)
                DataCells.Add(Cell);
            else
                DataCells.Insert(Column.Index, Cell);
        }

        /// <summary>
        /// 删除指定索引处的数据单元格（此方法不可让外部程序集使用）。
        /// </summary>
        /// <param name="Index">要删除的单元格索引。</param>
        internal void RemoveCell(int Index)
        {
            if (DataCells.Count < 1)
                return;
            string FieldName = DataCells[Index].Column.Name;
            DataCells.RemoveAt(Index);
            CellAddressHashTable.Remove(FieldName);
        }

        /// <summary>
        /// 向数据行中添加一个数据单元格。
        /// </summary>
        /// <param name="Column">该数据单元格所属的列对象。</param>
        /// <param name="Value"></param>
        protected DataCell Add(SpeedDataColumn Column, object Value)
        {
            if (Value != null && !(Value.GetType().Equals(Column.DataType)))
                throw (new Exception("Data type is inconsistent."));
            DataCell Cell = new DataCell();
            Cell.Column = Column;
            Cell.Value = Value;
            CellAddressHashTable.Add(Column.Name, Cell);
            DataCells.Add(Cell);
            return Cell;
        }

        /// <summary>
        /// 获取该行指定字段的值。
        /// </summary>
        /// <typeparam name="T">返回的数据类型。</typeparam>
        /// <param name="index">列索引。</param>
        /// <returns></returns>
        public T Field<T>(int index)
        {
            DataCell Cell = DataCells[index];
            if (Cell.Value == null || Cell.Value == DBNull.Value)
                return default(T);
            if (Cell.Value is T)
                return (T)(Cell.Value);
            if (typeof(T) == typeof(string))
                return (T)((object)(Cell.Value.ToString()));
            return (T)(Convert.ChangeType(Cell.Value, typeof(T)));
        }

        /// <summary>
        /// 获取该行指定字段的值。
        /// </summary>
        /// <typeparam name="T">返回的数据类型。</typeparam>
        /// <param name="ColumnName">列名称。</param>
        /// <returns></returns>
        public T Field<T>(string ColumnName)
        {
            DataCell Cell = FindCell(ColumnName);
            if (Cell == null)
                throw (new Exception(string.Format("列 {0} 不属于表。", ColumnName)));
            if (Cell.Value == null || Cell.Value == DBNull.Value)
                return default(T);
            if (Cell.Value is T)
                return (T)(Cell.Value);
            if (typeof(T) == typeof(string))
                return (T)((object)(Cell.Value.ToString()));
            return (T)(Convert.ChangeType(Cell.Value, typeof(T)));
        }

        #region INotifyPropertyChanged成员

        /// <summary>
        /// 在更改属性值时发生。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 用于触发 PropertyChanged 事件。
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region ICustomTypeDescriptor成员实现
        /*
         * 要将 SpeedDataTable 作为控件的 DataSource 绑定源必须实现 System.ComponentModel.ICustomTypeDescriptor
         * 并配合一个 System.ComponentModel.PropertyDescriptor 抽象类的实现，将行上的字段信息集合抽象化为SpeedDataRow 的属性成员
         * 若非如此，SpeedDataRow 数据行上的字段集合将无法被绑定所识别！
         * 
         */

        private PropertyDescriptorCollection properties;

        /// <summary>
        /// 返回该对象的特性集合。
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// 返回该对象的完整类名称。
        /// </summary>
        /// <returns></returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// 返回该对象的组件名称。
        /// </summary>
        /// <returns></returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// 返回该对象的类型转换器。
        /// </summary>
        /// <returns></returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// 返回该对象的默认事件。
        /// </summary>
        /// <returns></returns>
        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        /// <summary>
        /// 返回该对象的默认属性。
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        /// <summary>
        /// 返回该对象的类型编辑器。
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// 返回所有事件。
        /// </summary>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents()
        {
            return null;
        }

        /// <summary>
        /// 返回所有与特性相关的事件。
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return null;
        }

        /// <summary>
        /// 返回该对象的所有抽象化属性集合。
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties()
        {
            if (properties != null)
                return properties;
            properties = new PropertyDescriptorCollection(null);
            foreach (SpeedDataColumn Column in Table.Columns)
                properties.Add(new SpeedDataProperty(Column, null));
            return properties;
        }

        /// <summary>
        /// 返回该对象的所有抽象化属性集合。
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        /// <summary>
        /// 获取属性的所有者对象。
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
    }
}
