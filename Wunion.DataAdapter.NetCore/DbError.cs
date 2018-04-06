using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 追踪执行错误的错误信息对象类型。
    /// </summary>
    public class DbError
    {
        private string _Message;
        private string _CommandText;
        private string _ConnectionString;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DBAccess.ErrorInfo"/> 的对象实例。
        /// </summary>
        /// <param name="msg">错误信息。</param>
        /// <param name="command">引发错误的 SQL 命令原型。</param>
        /// <param name="conn">引发错误的连接信息。</param>
        public DbError(string msg, string command, string conn)
        {
            _Message = msg;
            _CommandText = command;
            _ConnectionString = conn;
        }

        /// <summary>
        /// 获取错误信息。
        /// </summary>
        public string Message
        {
            get { return _Message; }
        }

        /// <summary>
        /// 获取引发错误的 SQL 命令原型。
        /// </summary>
        public string CommandText
        {
            get { return _CommandText; }
        }

        /// <summary>
        /// 获取引发错误的连接信息。
        /// </summary>
        public string ConnectionString
        {
            get { return _ConnectionString; }
        }
    }
}
