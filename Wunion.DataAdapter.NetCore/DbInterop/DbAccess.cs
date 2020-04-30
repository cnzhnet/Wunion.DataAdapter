using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.DbInterop
{
    /// <summary>
    /// 与目录数据库交互的数据访问器基础对象类型。
    /// </summary>
    public abstract class DbAccess
    {
        protected DbError _Error;
        private object _SCOPE_IDENTITY;

        /// <summary>
        /// 创建对象实例。
        /// </summary>
        protected DbAccess()
        {
            ConnectionPool = null;
        }

        /// <summary>
        /// 获取或设置数据库连接字符串。
        /// </summary>
        public string ConnectionString
        {
            set;
            get;
        }

        /// <summary>
        /// 表示数据库连接池.
        /// </summary>
        public IDbConnectionPool ConnectionPool
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取或设置解释命令所需要的适配器。
        /// </summary>
        public ParserAdapter parserAdapter
        {
            get;
            set;
        }

        /// <summary>
        /// 获取错误信息。
        /// </summary>
        public DbError Error
        {
            get { return _Error; }
        }

        /// <summary>
        /// 获取用于取得当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public virtual string IdentityCommand
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 获取插入到当前会话的最后一个自增长字段的值。
        /// </summary>
        public object SCOPE_IDENTITY
        {
            get { return _SCOPE_IDENTITY; }
        }

        /// <summary>
        /// 连接池可时返回 true，否则返回 false.
        /// </summary>
        protected bool ConnectionPoolAvailable
        {
            get { return ConnectionPool != null && ConnectionPool.MaximumConnections > 0; }
        }

        /// <summary>
        /// 用于创建数据库连接.
        /// </summary>
        /// <returns></returns>
        protected abstract IDbConnection CreateConnection();

        /// <summary>
        /// 连接数据库，并返回一个 DbConnection 对象。
        /// </summary>
        /// <returns></returns>
        public IDbConnection Connect()
        {
            IDbConnection connection = null;
            if (ConnectionPoolAvailable)
            {
                connection = ConnectionPool.GetConnection(CreateConnection);
            }
            else
            {
                connection = CreateConnection();
                connection.Open();
            }
            return connection;
        }

        /// <summary>
        /// 创建要对数据库执行 SQL 命令或存储过程的对象。
        /// </summary>
        /// <returns></returns>
        public virtual IDbCommand CreateDbCommand()
        {
            return null;
        }

        /// <summary>
        /// 必须在所有SQL命令执行完成后并且数据库连接关闭前调用。
        /// </summary>
        /// <param name="DbCommand">会话的 SqlCommand 对象。</param>
        protected void QueryLastIdentity(IDbCommand DbCommand)
        {
            if (string.IsNullOrEmpty(IdentityCommand))
                return;
            try
            {
                DbCommand.CommandText = IdentityCommand;
                if (DbCommand.Connection.State == ConnectionState.Closed)
                    DbCommand.Connection.Open();
                _SCOPE_IDENTITY = DbCommand.ExecuteScalar();
            }
            catch
            {
                _SCOPE_IDENTITY = null;
            }
        }

        /// <summary>
        /// 开启或关闭指定表中自增长字段值的插入操作。
        /// </summary>
        /// <param name="table">表名。</param>
        /// <param name="enabled">是否启用（为true时启用，若要关闭则为false）。</param>
        public virtual void IdentityInsert(string table, bool enabled)
        {
            // 此方法由子类实现。
        }

        /// <summary>
        /// 执行指定的 SQL 命令，并返回受影响的记录数。
        /// </summary>
        /// <param name="Command">要执行的命令。</param>
        /// <returns></returns>
        public virtual int ExecuteNoneQuery(CommandBuilder Command)
        {
            return -1;
        }

        /// <summary>
        /// 指行指定的查询命令，并返回数据集。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public SpeedDataTable ExecuteQuery(CommandBuilder Command)
        {
            IDataReader Rd = ExecuteReader(Command);
            if (Rd == null)
                return null;
            SpeedDataTable dt = new SpeedDataTable();
            using (Rd)
            {
                string FieldName;
                int i = 0;
                // 创建列集合。
                if (Rd.FieldCount > 0)
                {
                    for (; i < Rd.FieldCount; ++i)
                    {
                        FieldName = Rd.GetName(i);
                        dt.Columns.Add(FieldName, Rd.GetFieldType(i));
                    }
                }

                // 填充所有行。
                while (Rd.Read())
                {
                    SpeedDataRow Row = new SpeedDataRow(dt);
                    for (i = 0; i < Rd.FieldCount; ++i)
                        Row[i] = Rd.GetValue(i);
                    dt.Add(Row);
                }
                Rd.Close();
            }
            return dt;
        }

        /// <summary>
        /// 指行指定的查询命令，并返回一个 DataTable 数据集。
        /// </summary>
        /// <param name="Command">要执行的命令。</param>
        /// <returns></returns>
        public virtual DataTable QueryDataTable(CommandBuilder Command)
        {
            return null;
        }

        /// <summary>
        /// 执行指定的查询，并返回该查询对应的动态实体对象数据集合.
        /// </summary>
        /// <param name="Command">要执行的查询命令对象.</param>
        /// <exception cref="Exception">在查询过程中产生错误时引发此异常.</exception>
        /// <returns>查询对应的动态实体对象数据集合.</returns>
        public List<dynamic> ExecuteDynamicEntity(CommandBuilder Command)
        {
            IDataReader Rd = ExecuteReader(Command);
            if (Rd == null)
            {
                string error;
                if (this.Error == null)
                    error = "在查询数据库时产生了未知的错误.";
                else
                    error = this.Error.Message;
                throw new Exception(error);
            }
            List<dynamic> dataCollection = new List<dynamic>();
            using (Rd)
            {
                DynamicEntity entity;
                int i = 0;
                while (Rd.Read())
                {
                    entity = new DynamicEntity();
                    for (i = 0; i < Rd.FieldCount; ++i)
                        entity.SetPropertyValue(Rd.GetName(i), Rd.GetValue(i), Rd.GetFieldType(i));
                    dataCollection.Add(entity);
                }
                Rd.Close();
            }
            return dataCollection;
        }

        /// <summary>
        /// 执行指定的查询命令，并返回相应的数据读取器。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(CommandBuilder Command)
        {
            return null;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public virtual object ExecuteScalar(CommandBuilder Command)
        {
            return null;
        }

        /// <summary>
        /// 创建SQL命令参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns></returns>
        public virtual IDbDataParameter CreateParameter(string parameterName, object value)
        {
            return null;
        }

        /// <summary>
        /// 创建 SQL 命令参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        /// <returns></returns>
        public virtual IDbDataParameter CreateParameter(string parameterName, object value, System.Data.ParameterDirection direction)
        {
            return null;
        }

        /// <summary>
        /// 清除以前产生的错误信息。
        /// </summary>
        protected void ClearError()
        {
            _Error = null;
        }
    }
}
