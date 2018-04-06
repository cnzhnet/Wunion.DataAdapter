using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace Wunion.DataAdapter.Kernel.DataCollection
{
    /// <summary>
    /// 极速数据表集合对象。
    /// </summary>
    [Serializable]
    public class SpeedDataTable : System.ComponentModel.BindingList<SpeedDataRow>, IDisposable
    {
        private SpeedDataColumnCollection _Columns;


        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataTable"/> 的对象实例。
        /// </summary>
        public SpeedDataTable() : base()
        {
            Initialize();
        }

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataTable"/> 的对象实例。
        /// </summary>
        /// <param name="tabName"></param>
        public SpeedDataTable(string tabName) : base()
        {
            Initialize();
            TableName = tabName;
        }

        /// <summary>
        /// 获取或设置表名称。
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取该表的数据列对象。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SpeedDataColumnCollection Columns
        {
            get { return _Columns; }
        }

        /// <summary>
        /// 初化 <see cref="SpeedDataTable"/> 对象。
        /// </summary>
        protected virtual void Initialize()
        {
            _IsDisposed = false;
            _Columns = new SpeedDataColumnCollection(this);
            _Columns.DataColumnAdded += DataColumns_DataColumnAdded;
            _Columns.DataColumnRemoved += DataColumns_DataColumnRemoved;
        }        

        /// <summary>
        /// 当在数据列集合中添加一个新列时要进行的处理。
        /// </summary>
        /// <param name="sender">触发事件的表。</param>
        /// <param name="Column">被添加的数据列对象。</param>
        private void DataColumns_DataColumnAdded(SpeedDataTable sender, SpeedDataColumn Column)
        {
            if (this.Count < 1)
                return;
            // 当列被加入时，所有行的该列索引处都应该插入一个单元格。否则单元格与列将会错位。
            foreach (SpeedDataRow Row in this)
            {
                Row.CreateNewCell(Column);
                Row[Column.Index] = Column.DefaultValue;
            }
        }

        /// <summary>
        /// 当某个列被从集合中删除时要进行的处理。
        /// </summary>
        /// <param name="sender">>触发事件的表。</param>
        /// <param name="Index">被删除的列索引。</param>
        private void DataColumns_DataColumnRemoved(SpeedDataTable sender, int Index)
        {
            if (this.Count < 1)
                return;
            // 所有行的该列单元格都要随之删除，否则列与单元格将会错位。
            foreach (SpeedDataRow Row in this)
                Row.RemoveCell(Index);
        }

        /// <summary>
        /// 对该表进行条件筛选，返回一个新的集合（该集合亦可作为绑定源）。
        /// </summary>
        /// <param name="Filter">用于筛选的 Lambda 表达式。</param>
        /// <returns></returns>
        public BindingList<SpeedDataRow> Select(Func<SpeedDataRow, bool> Filter)
        {
            IEnumerable<SpeedDataRow> Rows = this.Where(Filter);
            if (Rows == null)
                return null;
            return new BindingList<SpeedDataRow>(Rows.ToArray());
        }

        /// <summary>
        /// 返回指定字段为空时默认值
        /// </summary>
        /// <param name="Column">该字段的列对象。</param>
        /// <returns></returns>
        private object GetFieldNullValue(SpeedDataColumn Column)
        {
            //if (Column.DataType.IsValueType)
            //    return Activator.CreateInstance(Column.DataType);
            //else
            //    return "null";
            return "null";
        }

        /// <summary>
        /// 格式化字段值。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string FormatFieldValue(object val)
        {
            if (val.GetType().Equals(typeof(bool)))
                return val.ToString().ToLower();
            else
                return val.ToString();
        }

        /// <summary>
        /// 将数据集序列化为 JSON 文本。
        /// </summary>
        /// <returns></returns>
        public string SerializeJson()
        {
            StringBuilder buff = new StringBuilder("[");
            bool FristRow = true;
            foreach (SpeedDataRow Row in this)
            {
                if (FristRow)
                {
                    FristRow = false;
                    buff.Append("{");
                }
                else
                {
                    buff.Append(", {");
                }
                for (int i = 0; i < this.Columns.Count; ++i)
                {
                    buff.AppendFormat("\"{0}\": ", this.Columns[i].Name);
                    if (this.Columns[i].DataType == typeof(string) || this.Columns[i].DataType == typeof(DateTime))
                        buff.AppendFormat("\"{0}\"", (Row[i] == null || Row[i] == DBNull.Value) ? string.Empty : Row[i]);
                    else
                        buff.AppendFormat("{0}", (Row[i] == null || Row[i] == DBNull.Value) ? GetFieldNullValue(this.Columns[i]) : FormatFieldValue(Row[i]));
                    if (i < (this.Columns.Count - 1))
                        buff.Append(", ");
                }
                buff.Append(" }");

            }
            buff.Append("]");
            return buff.ToString();
        }

        /// <summary>
        /// 将指定的行序列化为 JSON 文本。
        /// </summary>
        /// <param name="Row">要序列化的数据行。</param>
        /// <returns></returns>
        public string SerializeJson(SpeedDataRow Row)
        {
            if (Row == null)
                return null;
            StringBuilder buff = new StringBuilder("{");
            if (Row.Table.Columns.Count > 0)
            {
                if (this.Columns[0].DataType == typeof(string) || this.Columns[0].DataType == typeof(DateTime))
                    buff.AppendFormat("\"{0}\": \"{1}\"", Row.Table.Columns[0].Name, (Row[0] == null || Row[0] == DBNull.Value) ? string.Empty : Row[0]);
                else
                    buff.AppendFormat("\"{0}\": {1}", Row.Table.Columns[0].Name, (Row[0] == null || Row[0] == DBNull.Value) ? string.Empty : Row[0]);
                if (Row.Table.Columns.Count > 1)
                {
                    for (int i = 1; i < Row.Table.Columns.Count; ++i)
                    {
                        if (this.Columns[i].DataType == typeof(string) || this.Columns[i].DataType == typeof(DateTime))
                            buff.AppendFormat(", \"{0}\": \"{1}\"", Row.Table.Columns[i].Name, (Row[i] == null || Row[i] == DBNull.Value) ? string.Empty : Row[i]);
                        else
                            buff.AppendFormat(", \"{0}\": {1}", Row.Table.Columns[i].Name, (Row[i] == null || Row[i] == DBNull.Value) ? GetFieldNullValue(this.Columns[i]) : FormatFieldValue(Row[i]));
                    }
                }
            }
            buff.Append("}");
            return buff.ToString();
        }

        #region IDisposable成员实现

        private bool _IsDisposed;

        /// <summary>
        /// 指示对象是否已被释放。
        /// </summary>
        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        /// <summary>
        /// 释放 <see cref="SpeedDataTable"/> 对象所占用的资源。
        /// </summary>
        /// <param name="disposing">手动调用则为 true，由对象终结器调用时为 false</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Columns.Clear();
                _Columns = null;
                TableName = null;
                this.Clear();
            }
            _IsDisposed = true;
            GC.Collect();
        }

        /// <summary>
        /// 释放 <see cref="SpeedDataTable"/> 对象所占用的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <see cref="SpeedDataTable"/> 的对象终结器。
        /// </summary>
        ~SpeedDataTable()
        {
            Dispose(false);            
        }

        #endregion
    }
}
