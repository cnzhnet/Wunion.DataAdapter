using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;
using System.Data;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.PostgreSQL
{
    /// <summary>
    /// PostgreSQL 的数据访问器.
    /// </summary>
    public class NpgsqlDbAccess : DbAccess
    {
        /// <summary>
        /// 获取用于取得当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public override string IdentityCommand
        {
            /*
             * select trim(replace(replace(column_default, 'nextval(''', ''), '''::regclass)', '')) 
	         *        as auto_identity 
             * from information_schema.columns 
             * where table_schema = 'public' and table_name = 'T_TEST1' and column_default like 'nextval%';
             * SELECT currval('auto_identity_seq');
             */
            get { return "SELECT LASTVAL();"; }
        }

        /// <summary>
        /// 用于获取删除表的命令.
        /// </summary>
        /// <param name="tableName">要删除的表名.</param>
        /// <param name="command">用于执行在获取删除表命令时可能需要进行的查询（如删除表后相关的约束残留信息的删除命令）.</param>
        /// <returns></returns>
        protected override string DropTableCommandText(string tableName, IDbCommand command)
        {
            StringBuilder buffers = new StringBuilder("DROP TABLE");
            buffers.AppendFormat(" \"{0}\";", tableName);
            // 查出删除表后会残留的自增长序列.
            command.CommandText = "select column_default from information_schema.columns where table_name = :tableName and position('nextval(' in column_default) > 0";
            command.Parameters.Add(CreateParameter("tableName", tableName));
            using (IDataReader Rd = command.ExecuteReader()) 
            {
                string nextVal = null;
                Match m = null;
                while (Rd.Read())
                {
                    nextVal = Rd["column_default"]?.ToString();
                    if (string.IsNullOrEmpty(nextVal))
                        continue;
                    m = Regex.Match(nextVal, "\\'\\\"[\\d\\W\\w_]+\\\"\\'", RegexOptions.ExplicitCapture);
                    if (m != null && !string.IsNullOrEmpty(m.Value))
                    {
                        nextVal = m.Value.Replace("\'", string.Empty);
                        buffers.AppendLine().AppendFormat("DROP SEQUENCE {0};", nextVal);
                    }
                }
                Rd.Close();
            }
            return buffers.ToString();
        }

        /// <summary>
        /// 用于创建数据库连接.
        /// </summary>
        /// <returns></returns>
        protected override IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw (new Exception("Connection string is invalid."));
            NpgsqlConnection DbConnection = new NpgsqlConnection();
            DbConnection.ConnectionString = ConnectionString;
            return DbConnection;
            /* 连接字符串示例.
             * Host=127.0.0.1;Username=postgres;Password=123456;Database=postgres
             */
        }

        /// <summary>
        /// 创建要对数据库执行 SQL 命令或存储过程的对象。
        /// </summary>
        /// <returns></returns>
        public override IDbCommand CreateDbCommand()
        {
            return new NpgsqlCommand();
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
                commander.CommandText = "select count(1) from information_schema.tables where table_name = :tableName";
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
            NpgsqlCommand DbCommand = new NpgsqlCommand();
            try
            {
                DbCommand.Connection = (NpgsqlConnection)Connect();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((NpgsqlParameter)p);
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
            NpgsqlDataAdapter DbAdapter = new NpgsqlDataAdapter(Command.Parsing(parserAdapter), (NpgsqlConnection)Connect());
            try
            {
                foreach (object p in Command.CommandParameters)
                    DbAdapter.SelectCommand.Parameters.Add((NpgsqlParameter)p);
                DbAdapter.SelectCommand.CommandType = Command.CommandType;
                DataTable table = new DataTable();
                DbAdapter.Fill(table);
                return table;
            }
            catch (Exception Ex)
            {
                _Error = new DbError(Ex.Message, DbAdapter.SelectCommand.CommandText, ConnectionString);
                return null;
            }
            finally
            {
                if (ConnectionPoolAvailable)
                {
                    ConnectionPool.ReleaseConnection(DbAdapter.SelectCommand.Connection);
                }
                else
                {
                    DbAdapter.SelectCommand.Connection.Close();
                    DbAdapter.SelectCommand.Connection.Dispose();
                }
                DbAdapter.SelectCommand.Parameters.Clear();
                DbAdapter.Dispose();
            }
        }

        /// <summary>
        /// 执行指定的查询命令，并返回相应的数据读取器。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public override IDataReader ExecuteReader(CommandBuilder Command)
        {
            ClearError();
            NpgsqlCommand DbCommand = new NpgsqlCommand();
            try
            {
                DbCommand.Connection = (NpgsqlConnection)Connect();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((NpgsqlParameter)p);
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
            NpgsqlCommand DbCommand = new NpgsqlCommand();
            try
            {
                DbCommand.Connection = (NpgsqlConnection)Connect();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((NpgsqlParameter)p);
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
            if (parameterName[0] != ':')
            {
                parameterName = string.Format(":{0}", parameterName);
            }
            return new NpgsqlParameter(parameterName, value);
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
