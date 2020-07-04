using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Npgsql;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;
using Wunion.DataAdapter.NetCore.PostgreSQL.CommandParser;

namespace Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser
{
    /// <summary>
    /// PostgreSQL 数据库的解释适配器.
    /// </summary>
    public class NpgsqlParserAdapter : ParserAdapter
    {
        /// <summary>
        /// 获取用于取得当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public override string IdentityCommand
        {
            get { return "SELECT LASTVAL();"; }
        }

        public override string ElemIdentifierL
        {
            get { return "\""; }
        }

        public override string ElemIdentifierR
        {
            get { return "\""; }
        }

        /// <summary>
        /// 初始化所有命令解释器.
        /// </summary>
        protected override void InitializeParsers()
        {
            base.InitializeParsers();
            RegisterParser(typeof(FunDescription), new NpgsqlFunParser(this));
            RegisterParser(typeof(LikeDescription), new NpgsqlLikeParser(this));
            RegisterParser(typeof(SelectBlock), new NpgsqlSelectBlockParser(this));
            RegisterParser(typeof(TableBuildDescription), new NpgsqlTableBuildParser(this));
        }

        /// <summary>
        /// 创建SQL命令参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns></returns>
        public override IDbDataParameter CreateDbParameter(string parameterName, object value)
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
        public override IDbDataParameter CreateDbParameter(string parameterName, object value, System.Data.ParameterDirection direction)
        {
            IDbDataParameter p = CreateDbParameter(parameterName, value);
            p.Direction = direction;
            return p;
        }
    }
}
