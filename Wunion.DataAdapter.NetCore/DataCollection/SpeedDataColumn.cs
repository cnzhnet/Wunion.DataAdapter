using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Wunion.DataAdapter.Kernel.DataCollection
{
    /// <summary>
    /// 表示数据列的对象类型。
    /// </summary>
    [Serializable]
    public class SpeedDataColumn
    {
        /// <summary>
        /// 获取或设置列名称。
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置该列的索引。
        /// </summary>
        internal int Index
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置该列的数据类型。
        /// </summary>
        public Type DataType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置默认值。
        /// </summary>
        public object DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataColumn"/> 的对象实例。
        /// </summary>
        public SpeedDataColumn()
        { }
    }

    /// <summary>
    /// 表示 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataTable"/> 的数据列集合。
    /// </summary>
    [Serializable]
    public class SpeedDataColumnCollection : IEnumerable
    {
        private SpeedDataTable _Table;
        private System.Collections.Hashtable ColumnsHashTable;
        private List<SpeedDataColumn> Items;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataColumnCollection"/> 的对象实例。
        /// </summary>
        public SpeedDataColumnCollection(SpeedDataTable Owner) : base()
        {
            _Table = Owner;
            ColumnsHashTable = new System.Collections.Hashtable();
            Items = new List<SpeedDataColumn>();
        }

        /// <summary>
        /// 获取该列集合所属的表。
        /// </summary>
        public SpeedDataTable Table
        {
            get { return _Table; }
        }

        /// <summary>
        /// 获取集合中的列数量。
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// 当数据列被添加到集合时触发。
        /// </summary>
        public event DataColumnCollectionEventHandler DataColumnAdded;

        /// <summary>
        /// 当数据列被从集合中删除时触发。
        /// </summary>
        public event DataColumnRemovedEventHandler DataColumnRemoved;

        /// <summary>
        /// 获取或设置指定索引的列。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns></returns>
        public SpeedDataColumn this[int index]
        {
            get { return Items[index]; }
        }

        /// <summary>
        /// 获取具有指定名称的列对象。
        /// </summary>
        /// <param name="Name">列名称。</param>
        /// <returns></returns>
        public SpeedDataColumn this[string Name]
        {
            get
            {   // 此处不用 ColumnsHashTable.ContainsKey(Name) 做判断，否则会搜索两次 Hash 表。
                // 在列存在的情况下 try....catch 并不影响程序性能，返而可以减少一次 Hash 表的搜索
                try
                {
                    return (SpeedDataColumn)ColumnsHashTable[Name];
                }
                catch
                {   // 隐藏 Hash 表的搜索异常信息，重新抛出更具说问题说明性的异常。
                    throw new Exception(string.Format("列 {0} 不属于表。", Name));
                }
            }
        }

        /// <summary>
        /// 用于触发 DataColumnAdded 事件。
        /// </summary>
        /// <param name="Column">与 DataColumnAdded 事件相关的数据列对象。</param>
        protected virtual void OnDataColumnAdded(SpeedDataColumn Column)
        {
            if (DataColumnAdded != null)
                DataColumnAdded(Table, Column);
        }

        /// <summary>
        /// 用于触发 DataColumnsRemoved 事件。
        /// </summary>
        /// <param name="Index">被删除的列索引。</param>
        protected virtual void OnDataColumnRemoved(int Index)
        {
            if (DataColumnRemoved != null)
                DataColumnRemoved(Table, Index);
        }

        /// <summary>
        /// 添加一个新列到集合结尾。
        /// </summary>
        /// <param name="item">新的列对象。</param>
        public void Add(SpeedDataColumn item)
        {
            if (item == null)
                return;
            if (ColumnsHashTable.ContainsKey(item.Name))
                return;
            
            ColumnsHashTable.Add(item.Name, item);
            Items.Add(item);
            item.Index = Items.Count - 1;
            OnDataColumnAdded(item);
        }

        /// <summary>
        /// 在集合中的指定位置插入一个新的元素。
        /// </summary>
        /// <param name="index">在该索引处插入元素。</param>
        /// <param name="item">新插入的元素对象。</param>
        public void Insert(int index, SpeedDataColumn item)
        {
            if (item == null)
                return;
            if (ColumnsHashTable.ContainsKey(item.Name))
                return;

            item.Index = index;
            Items.Insert(index, item);
            ColumnsHashTable.Add(item.Name, item);
            RearrangeColumnIndex(index + 1);
            OnDataColumnAdded(item);
        }

        /// <summary>
        /// 添加一个数据列。
        /// </summary>
        /// <param name="Name">列名称。</param>
        /// <param name="DataType">该列的数据类型。</param>
        /// <param name="defaultValue">该列的默认值。</param>
        public void Add(string Name, Type DataType, object defaultValue = null)
        {
            if (ColumnsHashTable.ContainsKey(Name))
                return;

            SpeedDataColumn Column = new SpeedDataColumn();
            Column.Name = Name;            
            Column.DataType = DataType;
            Column.DefaultValue = defaultValue;
            ColumnsHashTable.Add(Column.Name, Column);
            Items.Add(Column);
            Column.Index = Items.Count - 1;
            OnDataColumnAdded(Column);
        }

        /// <summary>
        /// 重新整理各个列的索引。
        /// </summary>
        /// <param name="beginIndex">从该索引处开始向后整理。</param>
        protected void RearrangeColumnIndex(int beginIndex)
        {
            if (beginIndex >= Items.Count)
                return;
            for (int i = beginIndex; beginIndex < Items.Count; ++i)
                Items[i].Index = i;
        }

        /// <summary>
        /// 从集合中移除一个元素。
        /// </summary>
        /// <param name="item">要移除的元素。</param>
        /// <returns></returns>
        public void Remove(SpeedDataColumn item)
        {
            int Index = item.Index;
            Items.Remove(item);
            ColumnsHashTable.Remove(item.Name);
            if (Index < (Items.Count - 1))
                RearrangeColumnIndex(Index + 1);
            OnDataColumnRemoved(Index);
        }

        /// <summary>
        /// 从集合中删除指定索引处的元素。
        /// </summary>
        /// <param name="index">删除该索引处的元素。</param>
        public void RemoveAt(int index)
        {
            string Name = Items[index].Name;
            Items.RemoveAt(index);
            ColumnsHashTable.Remove(Name);
            RearrangeColumnIndex(index + 1);
            OnDataColumnRemoved(index);
        }

        /// <summary>
        /// 清除集合内的所有列。
        /// </summary>
        public void Clear()
        {
            ColumnsHashTable.Clear();
            Items.Clear();
        }

        /// <summary>
        /// 返回一个遍历所有列的迭代器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                yield return Items[i];
            }
        }
    }

    /// <summary>
    /// 用于执行数据列触发事件的委托。
    /// </summary>
    /// <param name="sender">触发事件的对象。</param>
    /// <param name="Column">与事件相关的数据列对象。</param>
    public delegate void DataColumnCollectionEventHandler(SpeedDataTable sender, SpeedDataColumn Column);

    /// <summary>
    /// 用于执行数据列删除后触发事件的委托。
    /// </summary>
    /// <param name="sender">触发事件的对象。</param>
    /// <param name="Index">被删除的列索引。</param>
    public delegate void DataColumnRemovedEventHandler(SpeedDataTable sender, int Index);
}
