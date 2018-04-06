using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.SQLServer.CommandParser
{
    /// <summary>
    /// 适用于 Microsoft SQL Server 2008 R2 SP1 及以上版本的解释适配器。
    /// </summary>
    public class SqlServerParserAdapter : ParserAdapter
    {

        /// <summary>
        /// 获取用于取得当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public override string IdentityCommand
        {
            get { return "SELECT @@IDENTITY"; }
        }

        /// <summary>
        /// 初始化所有命令解释器.
        /// </summary>
        protected override void InitializeParsers()
        {
            base.InitializeParsers();
            Put(typeof(FunDescription), new SqlServerFunParser(this));
            Put(typeof(SelectBlock), new SqlServerSelectBlockParser(this));
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
            return new System.Data.SqlClient.SqlParameter(parameterName, value);
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
            IDbDataParameter p = CreateDbParameter(parameterName, value);
            p.Direction = direction;
            return p;
        }

        /// <summary>
        /// 返回开启或关闭指定表中自增长字段的命令文本。
        /// </summary>
        /// <param name="table">表名称。</param>
        /// <param name="enabled">开启为true，关闭则为false</param>
        /// <returns></returns>
        public override string IdentityInsertCommand(string table, bool enabled)
        {
            if (enabled)
                return string.Format("SET IDENTITY_INSERT [{0}] ON", table);
            else
                return string.Format("SET IDENTITY_INSERT [{0}] OFF", table);
        }
    }
}
