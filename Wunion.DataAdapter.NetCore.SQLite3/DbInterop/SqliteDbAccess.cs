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
        /// 连接数据库，并返回一个 DbConnection 对象。
        /// </summary>
        /// <returns></returns>
        public override IDbConnection GetConnection()
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
                DbCommand.Connection = (SqliteConnection)GetConnection();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((SqliteParameter)p);
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
            IDataReader Rd = ExecuteReader(Command);
            if (Rd == null)
                return null;
            DataTable dt = new DataTable();
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
            //dt.AcceptChanges();

            // 填充所有行。
            while (Rd.Read())
            {
                DataRow Row = dt.NewRow();
                for (i = 0; i < Rd.FieldCount; ++i)
                    Row[i] = Rd.GetValue(i);
                dt.Rows.Add(Row);
            }
            Rd.Close();
            Rd.Dispose();
            dt.AcceptChanges();
            return (T)dt;
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
                DbCommand.Connection = (SqliteConnection)GetConnection();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((SqliteParameter)p);
                DbCommand.CommandType = Command.CommandType;
                DbCommand.Connection.Open();
                SqliteDataReader DbReader = DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
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
            SqliteCommand DbCommand = new SqliteCommand();
            try
            {
                DbCommand.Connection = (SqliteConnection)GetConnection();
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                foreach (object p in Command.CommandParameters)
                    DbCommand.Parameters.Add((SqliteParameter)p);
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
