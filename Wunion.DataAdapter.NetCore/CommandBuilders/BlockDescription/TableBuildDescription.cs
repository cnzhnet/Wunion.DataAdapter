using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述 CREATE TABLE 命令的对象类型.
    /// </summary>
    public class TableBuildDescription : ParseDescription
    {
        private string tableName;
        private List<DbTableColumnDefinition> _ColumnDefinitions;

        /// <summary>
        /// 创建一个 <see cref="TableBuildDescription"/> 的对象实例.
        /// </summary>
        /// <param name="name">表名.</param>
        internal TableBuildDescription(string name)
        {
            tableName = name;
            _ColumnDefinitions = new List<DbTableColumnDefinition>();
        }

        /// <summary>
        /// 获取表名称.
        /// </summary>
        public string Name => tableName;

        /// <summary>
        /// 获取列定义信息.
        /// </summary>
        public List<DbTableColumnDefinition> ColumnDefinitions => _ColumnDefinitions;

        /// <summary>
        /// 定义该表的列.
        /// </summary>
        /// <returns></returns>
        public TableBuildDescription ColumnsDefine(params DbTableColumnDefinition[] definitions)
        {
            if (definitions != null)
                _ColumnDefinitions.AddRange(definitions);
            return this;
        }
    }

    /// <summary>
    /// 表示创建表时对列的定义信息.
    /// </summary>
    public class DbTableColumnDefinition
    {
        /// <summary>
        /// 创建一个 <see cref="DbTableColumnDefinition"/> 的对象实例.
        /// </summary>
        internal DbTableColumnDefinition()
        {
            NotNull = false;
            Default = null;
            Identity = null;
        }

        /// <summary>
        /// 列名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 列的数据类型.
        /// </summary>
        public GenericDbType DataType { get; set; }

        /// <summary>
        /// 数据的长度（某些数据类型指定该长度可能会导致错误）.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 不允许为空则为 true，否则应为 false .
        /// </summary>
        public bool NotNull { get; set; }

        /// <summary>
        /// 该列的值是否唯一.
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// 该列是否为主键（将多个列设定为主键时将自动产生联合主键）.
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 自动增长设置.
        /// </summary>
        public DbColumnIdentity Identity { get; set; }

        /// <summary>
        /// 列的默认值（要指定当前日期时间为默认值时可使用 Fun.Now() 函数，若为所有二进制类型的列指定默认值将引发异常）.
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        /// 外键设置.
        /// </summary>
        public DbForeignKey ForeignKey { get; set; }

        /// <summary>
        /// 创建一个新的列定义.
        /// </summary>
        /// <param name="name">列名称.</param>
        /// <param name="dataType">数据类型.</param>
        /// <param name="size">数据的长度（某些数据类型指定该长度可能会导致错误）.</param>
        /// <param name="notNull">不允许为空则为 true，否则应为 false .</param>
        /// <param name="unique">该列的值是否唯一.</param>
        /// <param name="pk">该列是否为主键（将多个列设定为主键时将自动产生联合主键）.</param>
        /// <param name="Default">列的默认值（要指定当前日期时间为默认值时可使用 Fun.Now() 函数，若为所有二进制类型的列指定默认值将引发异常）.</param>
        /// <param name="identity">自动增长设置.</param>
        /// <param name="fk">外键设置.</param>
        /// <returns></returns>
        public static DbTableColumnDefinition New(string name, GenericDbType dataType, int size = 0, bool notNull = false, bool unique = false, bool pk = false, object Default = null, DbColumnIdentity identity = null, DbForeignKey fk = null)
        {
            return new DbTableColumnDefinition { 
                Name = name, 
                DataType = dataType, 
                Size = size, 
                NotNull = notNull, 
                Default = Default, 
                Identity = identity, 
                PrimaryKey = pk, 
                ForeignKey = fk,
                Unique = unique 
            };
        }
    }

    /// <summary>
    /// 表示数据库列的自动增长对象信息.
    /// </summary>
    public class DbColumnIdentity
    {
        /// <summary>
        /// 创建一个 <see cref="DbColumnIdentity"/> 的对象实例.
        /// </summary>
        /// <param name="m">自增长的初始值.</param>
        /// <param name="n">自动增长的增量值.</param>
        public DbColumnIdentity(int m, int n)
        {
            InitValue = m;
            Increment = n;
        }

        /// <summary>
        /// 自增长的初始值.
        /// </summary>
        public int InitValue { get; private set; }

        /// <summary>
        /// 自动增长的增量值.
        /// </summary>
        public int Increment { get; private set; }
    }

    /// <summary>
    /// 表示数据库的外键约束信息.
    /// </summary>
    public class DbForeignKey
    {
        /// <summary>
        /// 创建一个 <see cref="DbForeignKey"/> 的对象实例.
        /// </summary>
        /// <param name="table">外键关联的表名.</param>
        /// <param name="column">外键关联表的字段名.</param>
        /// <param name="update">主表字段更新时的处理行为（具体设置请参考各类数据库的外键设置料资）.</param>
        /// <param name="delete">主表字段删除时的处理行为（具体设置请参考各类数据库的外键设置料资）.</param>
        public DbForeignKey(string table, string column, string update = "CASCADE", string delete = "CASCADE")
        {
            this.Table = table;
            this.Column = column;
            OnUpdate = update;
            OnDelete = delete;
        }

        /// <summary>
        /// 外键关联的表名.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 外键关联表的字段名.
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 字段更新时的处理行为.
        /// </summary>
        public string OnUpdate { get; set; }

        /// <summary>
        /// 字段删除时的处理行为.
        /// </summary>
        public string OnDelete { get; set; }
    }
}
