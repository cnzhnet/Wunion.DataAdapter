using System;
using System.Collections.Generic;
using System.Text;
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
        /// 连接数据库，并返回一个 DbConnection 对象。
        /// </summary>
        /// <returns></returns>
        public override IDbConnection GetConnection()
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
                DbCommand.Connection = (NpgsqlConnection)GetConnection();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((NpgsqlParameter)p);
                DbCommand.CommandType = Command.CommandType;
                DbCommand.Connection.Open();
                int result = DbCommand.ExecuteNonQuery();
                if (result > 0)
                    QueryLastIdentity(DbCommand);
                return result;
            }
            catch (Exception Ex)
            {
                _Error = new DbError(Ex.Message, DbCommand.CommandText, ConnectionString);
                return -1;
            }
            finally
            {
                DbCommand.Connection.Close();
                DbCommand.Connection.Dispose();
                DbCommand.Parameters.Clear();
                DbCommand.Dispose();
            }
        }

        /// <summary>
        /// 指行指定的查询命令，并返回数据集。
        /// </summary>
        /// <param name="Command">要执行的查询。</param>
        /// <returns></returns>
        public override T ExecuteQuery<T>(CommandBuilder Command)
        {
            if (!typeof(T).Equals(typeof(DataTable)))
                throw (new Exception("目标类型只能是 System.Data.DataTable."));
            ClearError();
            NpgsqlDataAdapter DbAdapter = new NpgsqlDataAdapter(Command.Parsing(parserAdapter), (NpgsqlConnection)GetConnection());
            try
            {
                foreach (object p in Command.CommandParameters)
                    DbAdapter.SelectCommand.Parameters.Add((NpgsqlParameter)p);
                DbAdapter.SelectCommand.CommandType = Command.CommandType;
                DataTable dt = new DataTable();
                DbAdapter.Fill(dt);
                return (T)dt;
            }
            catch (Exception Ex)
            {
                _Error = new DbError(Ex.Message, DbAdapter.SelectCommand.CommandText, ConnectionString);
                return null;
            }
            finally
            {
                DbAdapter.SelectCommand.Connection.Close();
                DbAdapter.SelectCommand.Connection.Dispose();
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
                DbCommand.Connection = (NpgsqlConnection)GetConnection();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((NpgsqlParameter)p);
                DbCommand.CommandType = Command.CommandType;
                DbCommand.Connection.Open();
                NpgsqlDataReader DbReader = DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                return DbReader;
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
                DbCommand.Connection = (NpgsqlConnection)GetConnection();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((NpgsqlParameter)p);
                DbCommand.CommandType = Command.CommandType;
                DbCommand.Connection.Open();
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
                DbCommand.Connection.Close();
                DbCommand.Connection.Dispose();
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
