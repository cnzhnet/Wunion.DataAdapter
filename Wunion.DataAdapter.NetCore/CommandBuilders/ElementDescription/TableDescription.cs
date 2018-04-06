using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述表对象的类型。
    /// </summary>
    public class TableDescription : ParseDescription
    {
        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.TableDescription"/> 的对象实例。
        /// </summary>
        public TableDescription()
        { }

        /// <summary>
        /// 获或设置表名称。
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置表的别名（在查询时可能会用到）
        /// </summary>
        public string Aliases
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 用于描述一个 LEFT JOIN 子句的类型。
    /// </summary>

    public class LeftJoinDescription : ParseDescription
    {
        private TableDescription _Table;
        private List<object> _OnDescription;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.LeftJoinDescription"/> 的对象实例。
        /// </summary>
        public LeftJoinDescription()
        {
            _OnDescription = new List<object>();
        }

        /// <summary>
        /// 获取或设置 LEFT JOIN 子句中的表信息。
        /// </summary>
        public TableDescription Table
        {
            get { return _Table; }
            set { _Table = value; }
        }

        /// <summary>
        /// 获取或设置 LEFT JOIN 子句中的 ON 条件段。
        /// </summary>
        public List<object> OnDescription
        {

            get { return _OnDescription; }
            set { _OnDescription = value; }
        }

        /// <summary>
        /// 添加 ON 子片段。
        /// </summary>
        /// <param name="Conditions">ON 条件片段。</param>
        public LeftJoinDescription ON(params object[] Conditions)
        {
            _OnDescription.AddRange(Conditions);
            return this;
        }        
    }

    /// <summary>
    /// 用于表或多表查询相关描述对象的快捷创建器。
    /// </summary>
    public static class fm
    {
        /// <summary>
        /// 创建一个表信息描述对象。
        /// </summary>
        /// <param name="name">表名称。</param>
        /// <param name="aliases">表别名（可以没有）。</param>
        /// <returns></returns>
        public static TableDescription Table(string name, string aliases)
        {
            TableDescription tab = new TableDescription();
            tab.Name = name;
            tab.Aliases = aliases;
            return tab;
        }

        /// <summary>
        /// 创建一个表信息描述对象。
        /// </summary>
        /// <param name="name">表名称。</param>
        /// <returns></returns>
        public static TableDescription Table(string name)
        {
            return Table(name, string.Empty);
        }

        /// <summary>
        /// 创建一个 LEFT JOIN 子句的信息描述对象。
        /// </summary>
        /// <param name="tab">LEFT JOIN 子句中的表。</param>
        /// <returns></returns>
        public static LeftJoinDescription LeftJoin(TableDescription tab)
        {
            LeftJoinDescription JeftJoin = new LeftJoinDescription();
            JeftJoin.Table = tab;
            return JeftJoin;
        }

        /// <summary>
        /// 创建一个 LEFT JOIN 子句的信息描述对象。
        /// </summary>
        /// <param name="name">表名称。</param>
        /// <param name="aliases">表别名（可以没有）。</param>
        /// <returns></returns>
        public static LeftJoinDescription LeftJoin(string name, string aliases)
        {
            LeftJoinDescription JeftJoin = new LeftJoinDescription();
            JeftJoin.Table = Table(name, aliases);
            return JeftJoin;
        }
    }
}
