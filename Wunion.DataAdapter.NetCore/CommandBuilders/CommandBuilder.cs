using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 命令构建器的基础对象类型（由此派生普通的 SQL 命令构建器或存储过程构建器）。
    /// </summary>
    public abstract class CommandBuilder
    {
        protected IDescription CommandDescription;
        protected List<IDbDataParameter> _CommandParameters;
        private bool _IsParsed;
        protected string CommandText; // 用于缓存解释器返回的目标 SQL 命令。
        protected string _IdentityCommand;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.CommandBuilder"/> 的对象类型。由派生类型继承。
        /// </summary>
        protected CommandBuilder()
        {
            _CommandParameters = new List<IDbDataParameter>();
            _IsParsed = false;
            CommandText = string.Empty;
        }

        /// <summary>
        /// 获取命令的参数集合（该集合的内容将在调用 Parsing 方法后产生）。
        /// </summary>
        public List<IDbDataParameter> CommandParameters
        {
            get { return _CommandParameters; }
        }

        /// <summary>
        /// 获取一个值，该值指示命令是否已经被解释过。
        /// </summary>
        public bool IsParsed
        {
            get { return _IsParsed; }
        }

        /// <summary>
        /// 获取该构建器所构建的命令类型。
        /// </summary>
        public virtual System.Data.CommandType CommandType
        {
            get { return System.Data.CommandType.Text; }
        }

        /// <summary>
        /// 获取用于查询当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public string IdentityCommand
        {
            get { return _IdentityCommand; }
        }

        /// <summary>
        /// 重置命令构建器。
        /// </summary>
        protected void ResetCommandBuilder()
        {
            _CommandParameters.Clear();
            _IsParsed = false;
            _IdentityCommand = string.Empty;
            CommandText = string.Empty;
        }

        /// <summary>
        /// 解释整个命令，同时创建该命令应有的参数并返回命令文本。
        /// </summary>
        /// <param name="adapter">解释命令所需的适配器。</param>
        /// <returns></returns>
        public virtual string Parsing(ParserAdapter adapter)
        {
            // 防止多次调用此方法导致参数集合内的参数重复。
            // 如果参数重复将会导致读或写库失败。
            if (IsParsed)
                return CommandText;

            CommandDescription.DescriptionParserAdapter = adapter;
            if (CommandDescription.DescriptionParserAdapter == null)
                throw (new Exception("未初始化解释命令所需要的适配器。"));
            _IdentityCommand = adapter.IdentityCommand;
            ParserBase parser = CommandDescription.GetParser();
            CommandText = parser.Parsing(ref _CommandParameters);
            _IsParsed = true;
            return CommandText;
        }
    }
}
