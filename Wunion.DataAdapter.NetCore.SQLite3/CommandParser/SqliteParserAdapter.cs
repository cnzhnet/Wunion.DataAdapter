using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.Sqlite;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.SQLite3.CommandParser
{
    /// <summary>
    /// SQLite 数据库的解释适配器。
    /// </summary>
    public class SqliteParserAdapter : ParserAdapter
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.SQLite3.CommandParser.SqliteParserAdapter"/> 的对象实例.
        /// </summary>
        public SqliteParserAdapter() : base()
        { }

        /// <summary>
        ///  获取用于查询当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public override string IdentityCommand
        {
            get { return "select last_insert_rowid();"; }
        }

        /// <summary>
        /// 初始化所有命令解释器.
        /// </summary>
        protected override void InitializeParsers()
        {
            base.InitializeParsers();
            Put(typeof(FunDescription), new SqliteFunParser(this));
            Put(typeof(SelectBlock), new SqliteSelectBlockParser(this));
        }

        /// <summary>
        /// 创建命令中的参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns></returns>
        public override IDbDataParameter CreateDbParameter(string parameterName, object value)
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
        /// 创建命令中的参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        /// <returns></returns>
        public override IDbDataParameter CreateDbParameter(string parameterName, object value, ParameterDirection direction)
        {
            IDbDataParameter sp = CreateDbParameter(parameterName, value);
            sp.Direction = direction;
            return sp;
        }
    }
}
