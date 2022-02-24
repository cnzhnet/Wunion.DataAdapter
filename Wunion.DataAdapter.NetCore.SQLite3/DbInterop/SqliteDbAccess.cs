using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.Sqlite;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.DataCollection;

namespace Wunion.DataAdapter.Kernel.SQLite3
{
    /// <summary>
    /// 适用于 SQLite3 的数据访问器对象类型。
    /// </summary>
    public class SqliteDbAccess : DbAccess
    {
        /// <summary>
        /// 获取用于取得当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public override string IdentityCommand
        {
            get { return "select last_insert_rowid();"; }
        }

        /// <summary>
        /// 用于获取删除表的命令.
        /// </summary>
        /// <param name="tableName">要删除的表名.</param>
        /// <param name="command">用于执行在获取删除表命令时可能需要进行的查询（如删除表后相关的约束残留信息的删除命令）.</param>
        /// <returns></returns>
        protected override string DropTableCommandText(string tableName, IDbCommand command)
        {
            StringBuilder sb = new StringBuilder(string.Format("DROP TABLE [{0}];", tableName));
            sb.AppendLine(string.Format("DELETE FROM sqlite_sequence WHERE name = '{0}';", tableName));
            return sb.ToString();
        }

        /// <summary>
        /// 用于创建数据库连接.
        /// </summary>
        /// <returns></returns>
        protected override IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw (new Exception("Connection string is invalid."));
            SqliteConnection DbConnection = new SqliteConnection();
            DbConnection.ConnectionString = ConnectionString;
            return DbConnection;
        }

        /// <summary>
        /// 创建要对数据库执行 SQL 命令或存储过程的对象。
        /// </summary>
        /// <returns></returns>
        public override IDbCommand CreateDbCommand()
        {
            return new SqliteCommand();
        }

        /// <summary>
        /// 若指定名称的表在数据库中存在则返回 true，否则返回 false .
        /// </summary>
        /// <param name="tableName">表名称.</param>
        /// <param name="commander">在该 DbCommand 上执行查询（为空时则自动创建，默认值 null）.</param>
        /// <returns></returns>
        public override bool TableExists(string tableName, IDbCommand commander = null)
        {
            bool releaseCommander = false;
            if (commander == null)
            {
                commander = CreateDbCommand();
                commander.Connection = Connect();
                releaseCommander = true;
            }
            ClearError();
            try
            {
                commander.CommandText = "SELECT COUNT(*) FROM [SQLITE_MASTER] where [type] = 'table' and [tbl_name] = @tableName";
                commander.CommandType = CommandType.Text;
                commander.Parameters.Add(CreateParameter("tableName", tableName));
                object result = commander.ExecuteScalar();
                if (result == null)
                    return false;
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception Ex)
            {
                _Error = new DbError(Ex.Message, commander.CommandText, commander.Connection.ConnectionString);
                return false;
            }
            finally
            {
                if (releaseCommander)
                {
                    if (ConnectionPoolAvailable)
                    {
                        ConnectionPool.ReleaseConnection(commander.Connection);
                    }
                    else
                    {
                        commander.Connection.Close();
                        commander.Connection.Dispose();
                    }
                    commander.Parameters.Clear();
                    commander.Dispose();
                }
            }
        }

        /// <summary>
        /// 执行指定的 SQL 命令，并返回受影响的记录数。
        /// </summary>
        /// <param name="Command">要执行的命令。</param>
        /// <returns></returns>
        public override int ExecuteNoneQuery(CommandBuilder Command)
        {
            ClearError();
            SqliteCommand DbCommand = new SqliteCommand();
            try
            {
                DbCommand.Connection = (SqliteConnection)Connect();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((SqliteParameter)p);
                DbCommand.CommandType = Command.CommandType;
                int result = DbCommand.ExecuteNonQuery();
                if (result > 0)
                    QueryLastIdentity(DbCommand);
                return result <= 0 ? 1 : result;
            }
            catch (Exception Ex)
            {
                _Error = new DbError(Ex.Message, DbCommand.CommandText, ConnectionString);
                return -1;
            }
            finally
            {
                if (ConnectionPoolAvailable)
                {
                    ConnectionPool.ReleaseConnection(DbCommand.Connection);
                }
                else
                {
                    DbCommand.Connection.Close();
                    DbCommand.Connection.Dispose();
                }
                DbCommand.Parameters.Clear();
                DbCommand.Dispose();
            }
        }

        /// <summary>
        /// 指行指定的查询命令，并返回数据集。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public override DataTable QueryDataTable(CommandBuilder Command)
        {
            ClearError();
            IDataReader Rd = ExecuteReader(Command);
            if (Rd == null)
                return null;
            DataTable table = new DataTable();
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
                        table.Columns.Add(FieldName, Rd.GetFieldType(i));
                    }
                }
                // 填充所有行。
                while (Rd.Read())
                {
                    DataRow Row = table.NewRow();
                    for (i = 0; i < Rd.FieldCount; ++i)
                        Row[i] = Rd.GetValue(i);
                    table.Rows.Add(Row);
                }
                Rd.Close();
                table.AcceptChanges();
            }            
            return table;
        }

        /// <summary>
        /// 执行指定的查询命令，并返回相应的数据读取器。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public override IDataReader ExecuteReader(CommandBuilder Command)
        {
            ClearError();
            SqliteCommand DbCommand = new SqliteCommand();
            try
            {
                DbCommand.Connection = (SqliteConnection)Connect();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((SqliteParameter)p);
                DbCommand.CommandType = Command.CommandType;
                if (ConnectionPoolAvailable)
                {
                    DbaDataReader DbReader = new DbaDataReader(DbCommand.ExecuteReader(), DbCommand.Connection);
                    DbReader.Closed += (IDbConnection conn) => { ConnectionPool.ReleaseConnection(conn); };
                    return DbReader;
                }
                return DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception Ex)
            {
                _Error = new DbError(Ex.Message, DbCommand.CommandText, DbCommand.Connection.ConnectionString);
                return null;
            }
            finally
            {
                DbCommand.Parameters.Clear();
                DbCommand.Dispose();
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public override object ExecuteScalar(CommandBuilder Command)
        {
            ClearError();
            SqliteCommand DbCommand = new SqliteCommand();
            try
            {
                DbCommand.Connection = (SqliteConnection)Connect();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((SqliteParameter)p);
                DbCommand.CommandType = Command.CommandType;
                object result = DbCommand.ExecuteScalar();
                return result;
            }
            catch (Exception Ex)
            {
                _Error = new DbError(Ex.Message, DbCommand.CommandText, DbCommand.Connection.ConnectionString);
                return -1;
            }
            finally
            {
                if (ConnectionPoolAvailable)
                {
                    ConnectionPool.ReleaseConnection(DbCommand.Connection);
                }
                else
                {
                    DbCommand.Connection.Close();
                    DbCommand.Connection.Dispose();
                }
                DbCommand.Parameters.Clear();
                DbCommand.Dispose();
            }
        }

        /// <summary>
        /// 创建SQL命令参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            if (parameterName[0] != '@')
            {
                if (parameterName[0] == ':')
                    parameterName = parameterName.Remove(0, 1);
                parameterName = string.Format("@{0}", parameterName);
            }
            return new SqliteParameter(parameterName, value);
        }

        /// <summary>
        /// 创建 SQL 命令参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(string parameterName, object value, System.Data.ParameterDirection direction)
        {
            IDbDataParameter p = CreateParameter(parameterName, value);
            p.Direction = direction;
            return p;
        }
    }
}
