﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.DbInterop
{
    /// <summary>
    /// 在事务中使用的数据访问器。
    /// </summary>
    public class TransactionDbAccess
    {
        private IDbCommand DbCommand;
        private List<DbError> _Errors;
        private ParserAdapter _parserAdapter;
        private object _LastIdentity;
        protected DataEngine _Engine;

        /// <summary>
        /// 创建一个 <see cref="TransactionDbAccess"/> 的对象实例。
        /// </summary>
        /// <param name="cmd">与事务关联的 DbCommand 对象。</param>
        /// <param name="owner">创建该事务的引擎对象实例。</param>
        internal TransactionDbAccess(IDbCommand cmd, DataEngine owner)
        {
            DbCommand = cmd;
            _Engine = owner;
            _parserAdapter = owner.CommandParserAdapter;
            _Errors = new List<DbError>();
        }

        /// <summary>
        /// 获取创建该事务的引擎对象。
        /// </summary>
        public DataEngine Engine
        {
            get { return _Engine; }
        }

        /// <summary>
        /// 获取解析命令所需要的解释适配器。
        /// </summary>
        public ParserAdapter parserAdapter
        {
            get { return _parserAdapter; }
        }

        /// <summary>
        /// 或者在事务过程中所产生的错误信息列表。
        /// </summary>
        public List<DbError> Errors
        {
            get { return _Errors; }
        }

        /// <summary>
        /// 获取插入到当前会话的最后一个自增长字段的值。
        /// </summary>
        public object SCOPE_IDENTITY
        {
            get
            {
                QueryLastIdentity(parserAdapter);
                return _LastIdentity;
            }
        }

        /// <summary>
        /// 必须在所有SQL命令执行完成后并且数据库连接关闭前调用。
        /// </summary>
        /// <param name="adapter">解释命令所需的适配器。</param>
        private void QueryLastIdentity(ParserAdapter adapter)
        {
            if (string.IsNullOrEmpty(adapter.IdentityCommand))
                return;
            try
            {
                DbCommand.CommandText = adapter.IdentityCommand;
                if (DbCommand.Connection.State == ConnectionState.Closed)
                    DbCommand.Connection.Open();
                _LastIdentity = DbCommand.ExecuteScalar();
            }
            catch
            {
                _LastIdentity = null;
            }
        }

        /// <summary>
        /// 执行指定的 SQL 命令，并返回受影响的记录数。
        /// </summary>
        /// <param name="Command">CommandBuilder对象。</param>
        /// <returns></returns>
        public int ExecuteNoneQuery(CommandBuilder Command)
        {
            int result = -1;
            try
            {
                DbCommand.CommandText = Command.Parsing(parserAdapter);
                DbCommand.CommandType = Command.CommandType;
                if (Command.CommandParameters.Count > 0)
                {
                    foreach (IDbDataParameter p in Command.CommandParameters)
                        DbCommand.Parameters.Add(p);
                }
                result = DbCommand.ExecuteNonQuery();
                if (result <= 0)
                    result = 1;
            }
            catch (Exception Ex)
            {
                _Errors.Add(new DbError(Ex.Message, DbCommand.CommandText, DbCommand.Connection.ConnectionString));
            }
            finally
            {
                DbCommand.Parameters.Clear();
            }
            return result;
        }

        /// <summary>
        /// 执行指定的查询命令，并返回相应的数据读取器。
        /// </summary>
        /// <param name="command">要执行的查询的命令构建器.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandBuilder command)
        {
            IDataReader reader = null;
            try
            {
                DbCommand.CommandText = command.Parsing(parserAdapter);
                DbCommand.CommandType = command.CommandType;
                foreach (IDbDataParameter p in command.CommandParameters)
                    DbCommand.Parameters.Add(p);
                reader = DbCommand.ExecuteReader();
            }
            finally
            {
                DbCommand.Parameters.Clear();
            }
            return reader;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="command">要执行的查询。</param>
        /// <returns></returns>
        public object ExecuteScalar(CommandBuilder command)
        {
            object result = null;
            try
            {
                DbCommand.CommandText = command.Parsing(parserAdapter);
                DbCommand.CommandType = command.CommandType;
                foreach (IDbDataParameter p in command.CommandParameters)
                    DbCommand.Parameters.Add(p);
                result = DbCommand.ExecuteScalar();
            }
            finally
            {
                DbCommand.Parameters.Clear();
            }
            return result;
        }

        /// <summary>
        /// 若指定的表在数据库中存在则返回 true，否则返回 false .
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableExists(string tableName)
        {
            try
            {
                DbCommand.Parameters.Clear();
                return Engine.DBA.TableExists(tableName, DbCommand);
            }
            catch (Exception Ex)
            {
                _Errors.Add(new DbError(Ex.Message, DbCommand.CommandText, DbCommand.Connection.ConnectionString));
                return false;
            }
        }

        /// <summary>
        /// 从数据库中删除指定的表.
        /// </summary>
        /// <param name="tableName">表名称.</param>
        /// <returns></returns>
        public bool DropTable(string tableName)
        {
            try
            {
                Engine.DBA.DropTable(tableName, DbCommand);
                return true;
            }
            catch (Exception Ex)
            {
                _Errors.Add(new DbError(Ex.Message, DbCommand.CommandText, DbCommand.Connection.ConnectionString));
                return false;
            }
        }

        /// <summary>
        /// 开启或关闭指定表中自增长字段值的插入操作。
        /// </summary>
        /// <param name="table">表名。</param>
        /// <param name="enabled">是否启用（为true时启用，若要关闭则为false）。</param>
        public void IdentityInsert(string table, bool enabled)
        {
            string command = parserAdapter.IdentityInsertCommand(table, enabled);
            if (!string.IsNullOrEmpty(command))
            {
                try
                {
                    DbCommand.CommandText = command;
                    DbCommand.CommandType = CommandType.Text;
                    DbCommand.ExecuteNonQuery();
                }
                catch {
                }
            }
        }
    }
}
