using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 用于在一个连接上分批执行命令的提供程序.
    /// </summary>
    public class BatchCommander : IDisposable
    {
        private DataEngine Engine;
        private IDbCommand commander;

        /// <summary>
        /// 创建一个 <see cref="BatchCommander"/> 的对象实例.
        /// </summary>
        /// <param name="database">此提供程序使用的数据库引擎.</param>
        public BatchCommander(DataEngine database)
        {
            Engine = database;
            commander = Engine.DBA.CreateDbCommand();
            commander.Connection = Engine.DBA.Connect();
            if (commander.Connection.State != ConnectionState.Open)
                commander.Connection.Open();
        }

        /// <summary>
        /// 获取插入到当前会话的最后一个自增长字段的值。
        /// </summary>
        public object SCOPE_IDENTITY
        {
            get
            {
                if (string.IsNullOrEmpty(Engine.CommandParserAdapter.IdentityCommand))
                    return null;
                commander.CommandType = CommandType.Text;
                commander.CommandText = Engine.CommandParserAdapter.IdentityCommand;
                return commander.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行命令，并返回受影响记录数.
        /// </summary>
        /// <param name="command">要执行的命令的构建器对象.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbCommandBuilder command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            commander.Parameters.Clear();
            commander.CommandType = command.CommandType;
            commander.CommandText = command.Parsing(Engine.CommandParserAdapter);
            foreach (IDbDataParameter p in command.CommandParameters)
                commander.Parameters.Add(p);
            int result = commander.ExecuteNonQuery();
            if (result <= 0)
                return 1;
            return result;
        }

        /// <summary>
        /// 执行查询命令，返回查询结果中第一行第一列的值，并忽略所有其它的值.
        /// </summary>
        /// <param name="command">要执行的命令的构建器对象.</param>
        /// <returns>返回查询结果中第一行第一列的值，并忽略所有其它的值.</returns>
        public object QueryScalar(DbCommandBuilder command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            commander.Parameters.Clear();
            commander.CommandType = command.CommandType;
            commander.CommandText = command.Parsing(Engine.CommandParserAdapter);
            foreach (IDbDataParameter p in command.CommandParameters)
                commander.Parameters.Add(p);
            object result = commander.ExecuteScalar();            
            return result;
        }

        /// <summary>
        /// 执行查询命令，并返回一个数据读取器.
        /// </summary>
        /// <param name="command">要执行的命令的构建器对象.</param>
        /// <returns>返回一个数据读取器.</returns>
        public IDataReader ExecuteReader(DbCommandBuilder command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            commander.Parameters.Clear();
            commander.CommandType = command.CommandType;
            commander.CommandText = command.Parsing(Engine.CommandParserAdapter);
            foreach (IDbDataParameter p in command.CommandParameters)
                commander.Parameters.Add(p);
            return commander.ExecuteReader();
        }

        /// <summary>
        /// 若指定名称的表在数据库中存在则返回 true，否则返回 false.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableExists(string tableName)
        {
            commander.Parameters.Clear();
            return Engine.DBA.TableExists(tableName, commander);
        }

        /// <summary>
        /// 从数据库中删除指定的表.
        /// </summary>
        /// <param name="tableName">表名称.</param>
        public void DropTable(string tableName)
        {
            commander.Parameters.Clear();
            Engine.DBA.DropTable(tableName, commander);
        }

        #region IDisposable成员实现

        /// <summary>
        /// 释放对象所占用的资源.
        /// </summary>
        /// <param name="disposing">手动调用则为 true，由对象终结器调用时则为 false .</param>
        protected virtual void Dispose(bool disposing)
        {
            if (commander != null)
            {
                if (Engine.DBA.ConnectionPool != null && Engine.DBA.ConnectionPool.MaximumConnections > 0)
                {
                    Engine.DBA.ConnectionPool.ReleaseConnection(commander.Connection);
                }
                else
                {
                    commander.Connection.Close();
                    commander.Connection.Dispose();
                }
                commander.Parameters.Clear();                
                commander.Dispose();
            }
            commander = null;
        }

        /// <summary>
        /// 释放对象所占用的资源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 对象终结器（析构函数）.
        /// </summary>
        ~BatchCommander()
        {
            Dispose(false);
        }
        #endregion
    }
}
