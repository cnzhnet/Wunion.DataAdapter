using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 表示解释器适配器的基类型（它用于创建相应数据库对应的命令解释器）。
    /// </summary>
    public abstract class ParserAdapter
    {
        private Dictionary<Type, ParserBase> Parsers;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandParser.ParserAdapter"/> 的对象实例。
        /// </summary>
        public ParserAdapter()
        {
            Parsers = new Dictionary<Type, ParserBase>();
            InitializeParsers();
        }

        /// <summary>
        /// 获取用于查询当前会话的最后一个自增长字段值的命令。
        /// </summary>
        public virtual string IdentityCommand
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 表示命令元素的防保留字冲突左括符（不同的数据库可能不一样，可由子类重写返回其相应符号）。
        /// </summary>
        public virtual string ElemIdentifierL
        {
            get { return "["; }
        }

        /// <summary>
        /// 表示命令元素的防保留字冲突右括符（不同的数据库可能不一样，可由子类重写返回其相应符号）。
        /// </summary>
        public virtual string ElemIdentifierR
        {
            get { return "]"; }
        }

        /// <summary>
        /// 向适配器中压入一个解释器映射.
        /// </summary>
        /// <param name="forDescription"></param>
        /// <param name="parser"></param>
        protected void Put(Type forDescription, ParserBase parser)
        {
            if (Parsers.ContainsKey(forDescription))
                Parsers[forDescription] = parser;
            else
                Parsers.Add(forDescription, parser);

        }

        /// <summary>
        /// 获取指定的描述对象对应的解释器.
        /// </summary>
        /// <param name="forDescription">获取该类型对应的解释器.</param>
        /// <returns></returns>
        protected ParserBase Get(Type forDescription)
        {
            if (Parsers.ContainsKey(forDescription))
                return Parsers[forDescription];
            return null;
        }

        /// <summary>
        /// 初始化命令解释器.
        /// </summary>
        protected virtual void InitializeParsers()
        {
            Put(typeof(FieldDescription), new FieldParser(this));
            Put(typeof(TableDescription), new TableParser(this));
            Put(typeof(LeftJoinDescription), new LeftJoinParser(this));
            Put(typeof(ExpDescription), new ExpParser(this));
            Put(typeof(LogicAndDescription), new LogicAndParser(this));
            Put(typeof(LogicOrDescription), new LogicOrParser(this));
            Put(typeof(LogicNotDescription), new LogicNotParser(this));
            Put(typeof(LikeDescription), new LikeParser(this));
            Put(typeof(FunDescription), new FunParser(this));
            Put(typeof(AsElementDecsription), new AsElementParser(this));
            Put(typeof(DeleteBlock), new DeleteBlockParser(this));
            Put(typeof(FromBlock), new FromBlockParser(this));
            Put(typeof(GroupByBlock), new GroupByBlockParser(this));
            Put(typeof(InsertBlock), new InsertBlockParser(this));
            Put(typeof(OrderByBlock), new OrderByBlockParser(this));
            Put(typeof(SelectBlock), new SelectBlockParser(this));
            Put(typeof(SetBlock), new SetBlockParser(this));
            Put(typeof(UpdateBlock), new UpdateBlockParser(this));
            Put(typeof(WhereBlock), new WhereBlockParser(this));
            Put(typeof(GroupDescription), new GroupElementParser(this));
        }

        /// <summary>
        /// 通过 Descript 对象获取其对应的解释器。
        /// </summary>
        /// <param name="desObj">命令描述对象。</param>
        /// <returns></returns>
        public ParserBase GetParserByObject(IDescription desObj)
        {
            ParserBase parser = Get(desObj.GetType());
            if (parser != null)
                parser.Description = desObj;
            return parser;
        }

        /// <summary>
        /// 创建命令中的参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns></returns>
        public virtual IDbDataParameter CreateDbParameter(string parameterName, object value)
        {
            return null;
        }

        /// <summary>
        /// 创建命令中的参数。
        /// </summary>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        /// <returns></returns>
        public virtual IDbDataParameter CreateDbParameter(string parameterName, object value, System.Data.ParameterDirection direction)
        {
            return null;
        }

        /// <summary>
        /// 返回开启或关闭指定表中自增长字段的命令文本。
        /// </summary>
        /// <param name="table">表名称。</param>
        /// <param name="enabled">开启为true，关闭则为false</param>
        /// <returns></returns>
        public virtual string IdentityInsertCommand(string table, bool enabled)
        {
            return string.Empty;
            // 大多数数据库不需要进行任何操作，更多情况由子类重定返回具体的命令，例如：Microsoft SQL Server。
        }
    }
}
