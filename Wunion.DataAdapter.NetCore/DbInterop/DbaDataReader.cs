using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Wunion.DataAdapter.Kernel.DbInterop
{
    /// <summary>
    /// 用于支持数据访问器连接池回收机制的数据读取器.
    /// </summary>
    public sealed class DbaDataReader : IDataReader
    {
        /// <summary>
        /// 用于执行数据读取器关闭后的事件的委托.
        /// </summary>
        /// <param name="connection">与该读取器关联的数据库连接.</param>
        public delegate void ClosedEventHandler(IDbConnection connection);
        private IDataReader reader;
        private IDbConnection connection;

        /// <summary>
        /// 创建一个 <see cref="DbaDataReader"/> 的对象实例.
        /// </summary>
        /// <param name="innerReader">内置的基础数据读取器.</param>
        /// <param name="conn">与该数据读取器相关联的数据库连接.</param>
        public DbaDataReader(IDataReader innerReader, IDbConnection conn)
        {
            reader = innerReader;
            connection = conn;
        }

        /// <summary>
        /// 获取指定索引处的列的值.
        /// </summary>
        /// <param name="i">列索引.</param>
        /// <returns></returns>
        public object this[int i] => reader[i];

        /// <summary>
        /// 获取指定名称写的值.
        /// </summary>
        /// <param name="name">列名称.</param>
        /// <returns></returns>
        public object this[string name] => reader[name];

        /// <summary>
        /// 获取一个值，该值指示当前行的嵌套深度.
        /// </summary>
        public int Depth => reader.Depth;

        /// <summary>
        /// 获取一个值，该值指示读取器是否已经关闭.
        /// </summary>
        public bool IsClosed => reader.IsClosed;

        /// <summary>
        /// 获取通过执行SQL语句更改，插入或删除的行数.
        /// </summary>
        public int RecordsAffected => reader.RecordsAffected;

        /// <summary>
        /// 获取当前行的列数.
        /// </summary>
        public int FieldCount => reader.FieldCount;

        /// <summary>
        /// 该读取器被关闭后触发此事件.
        /// </summary>
        public event ClosedEventHandler Closed;

        /// <summary>
        /// 用于触发 <see cref="DbaDataReader.Closed"/> 事件.
        /// </summary>
        private void OnClosed()
        {
            if (Closed != null)
                Closed(connection);
        }

        /// <summary>
        /// 获取指定索引处的列的布尔值.
        /// </summary>
        /// <param name="i">列索引.</param>
        /// <returns></returns>
        public bool GetBoolean(int i) => reader.GetBoolean(i);

        /// <summary>
        /// 获取指定列的8位无符号整数（字节）值.
        /// </summary>
        /// <param name="i">列索引.</param>
        /// <returns></returns>
        public byte GetByte(int i) => reader.GetByte(i);

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

        public char GetChar(int i) => reader.GetChar(i);

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

        public IDataReader GetData(int i) => reader.GetData(i);

        public string GetDataTypeName(int i) => reader.GetDataTypeName(i);

        public DateTime GetDateTime(int i) => reader.GetDateTime(i);

        public decimal GetDecimal(int i) => reader.GetDecimal(i);

        public double GetDouble(int i) => reader.GetDouble(i);

        public Type GetFieldType(int i) => reader.GetFieldType(i);

        public float GetFloat(int i) => reader.GetFloat(i);

        public Guid GetGuid(int i) => reader.GetGuid(i);

        public short GetInt16(int i) => reader.GetInt16(i);

        public int GetInt32(int i) => reader.GetInt32(i);

        public long GetInt64(int i) => reader.GetInt64(i);

        public string GetName(int i) => reader.GetName(i);

        public int GetOrdinal(string name) => reader.GetOrdinal(name);

        public DataTable GetSchemaTable() => reader.GetSchemaTable();

        public string GetString(int i) => reader.GetString(i);

        public object GetValue(int i) => reader.GetValue(i);

        public int GetValues(object[] values) => reader.GetValues(values);

        public bool IsDBNull(int i) => reader.IsDBNull(i);

        public bool NextResult() => reader.NextResult();

        public bool Read() => reader.Read();
        
        /// <summary>
        /// 关闭该数据读取器.
        /// </summary>
        public void Close()
        {
            reader.Close();
            OnClosed();
        }

        #region IDisposable成员

        /// <summary>
        /// 释放对象占用的资源.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (reader == null)
                return;

            if (!reader.IsClosed)
                this.Close();
            reader.Dispose();
            reader = null;
        }

        /// <summary>
        /// 释放该对象所占用的资源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构函数.
        /// </summary>
        ~DbaDataReader()
        {
            Dispose(false);
        }
        #endregion
    }
}
