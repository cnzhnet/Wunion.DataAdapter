using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.MySQL.CommandParser
{
    /// <summary>
    /// MySQL 数据库的命令解释适配器.
    /// </summary>
    public class MySqlParserAdapter : ParserAdapter
    {
        /// <summary>
        /// 创建一个 <see cref="MySqlParserAdapter"/> 的对象实例.
        /// </summary>
        /// <param name="engine">MySQL 使用的引擎设置.</param>
        public MySqlParserAdapter(string engine = StorageEngine.INNODB) : base()
        {
            MysqlEngine = engine;
        }

        public override string IdentityCommand
        {
            get { return "SELECT LAST_INSERT_ID()"; }
        }

        public override string ElemIdentifierL
        {
            get
            {
                return "`";
            }
        }
        public override string ElemIdentifierR
        {
            get
            {
                return "`";
            }
        }

        /// <summary>
        /// 表示 MySQL 使用的引擎设置.
        /// </summary>
        internal string MysqlEngine { get; private set; }

        /// <summary>
        /// 初始化所有命令解释器.
        /// </summary>
        protected override void InitializeParsers()
        {
            base.InitializeParsers();
            RegisterParser(typeof(FunDescription), new MySqlFunParser(this));
            RegisterParser(typeof(LikeDescription), new MySqlLikeParser(this));
            RegisterParser(typeof(SelectBlock), new MySqlSelectBlockParser(this));
            RegisterParser(typeof(TableBuildDescription), new MySqlTableBuildParser(this));
        }

        /// <summary>
        /// 创建命令中的参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns></returns>
        public override IDbDataParameter CreateDbParameter(string parameterName, object value)
        {
            if (parameterName[0] != '?')
            {
                if (parameterName[0] == ':')
                    parameterName = parameterName.Remove(0, 1);
                parameterName = string.Format("?{0}", parameterName); MySqlParameter sp = new MySqlParameter();
            }
            return new MySqlParameter(parameterName, value);
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
