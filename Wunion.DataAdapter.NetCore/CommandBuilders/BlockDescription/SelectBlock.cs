using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 用于描述 SELECT 命令的对象类型。
    /// </summary>
    public class SelectBlock : ParseDescription
    {
        private PageContext _Pager;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.SelectBlock"/> 的对象类型。
        /// </summary>
        public SelectBlock()
        {
            Blocks = new List<IDescription>();
            _Pager = null;
        }

        /// <summary>
        /// 获取或设置 SELECT 的子句。
        /// </summary>
        public List<IDescription> Blocks
        {
            get;
            set;
        }

        /// <summary>
        /// 获取分页信息。
        /// </summary>
        public PageContext Pager
        {
            get { return _Pager; }
        }

        /// <summary>
        /// 向 SELECT 语句中添加元素。
        /// </summary>
        /// <param name="element">要添加的元素对象。</param>
        public void AddElement(IDescription element)
        {
            Blocks.Add(element);
        }

        /// <summary>
        /// 设置 SELECT 语句的 FROM 子句（适用于单表查询）。
        /// </summary>
        /// <param name="tableName">要查询的表名称。</param>
        /// <returns></returns>
        public SelectBlock From(string tableName)
        {
            FromBlock fb = new FromBlock();
            fb.Content.Add(fm.Table(tableName));
            Blocks.Add(fb);
            return this;
        }

        /// <summary>
        /// 设置 SELECT 语句的 FROM 子句（适用于多表查询）。
        /// </summary>
        /// <param name="content">FROM段的内容。</param>
        /// <returns></returns>
        public SelectBlock From(params IDescription[] content)
        {
            FromBlock fb = new FromBlock();
            fb.Content.AddRange(content);
            Blocks.Add(fb);
            return this;
        }

        /// <summary>
        /// 设置 SELECT 的 WHERE 子句。
        /// </summary>
        /// <param name="content">WHERE条件内容。</param>
        /// <returns></returns>
        public SelectBlock Where(params object[] content)
        {
            WhereBlock wb = new WhereBlock();
            wb.Content.AddRange(content);
            Blocks.Add(wb);
            return this;
        }

        /// <summary>
        /// 设置 SELECT 的 GROUP BY 子句。
        /// </summary>
        /// <param name="fields">字段列表。</param>
        /// <returns></returns>
        public SelectBlock GroupBy(params FieldDescription[] fields)
        {
            GroupByBlock gb = new GroupByBlock();
            gb.Fields.AddRange(fields);
            Blocks.Add(gb);
            return this;
        }

        /// <summary>
        /// 获取或设置 SELECT 的 ORDER BY 子句。
        /// </summary>
        /// <param name="field">排序字段。</param>
        /// <param name="sort">排序模式。</param>
        /// <returns></returns>
        public SelectBlock OrderBy(FieldDescription field, OrderByMode sort)
        {
            OrderByBlock ob = new OrderByBlock();
            ob.Field = field;
            ob.Sort = sort;
            Blocks.Add(ob);
            return this;
        }

        /// <summary>
        /// 为查询设置分页信息。
        /// </summary>
        /// <param name="page_size">每页的条目数量。</param>
        /// <param name="current_page">当前页（从第一页开始）。</param>
        /// <returns></returns>
        public SelectBlock Paging(int page_size, int current_page = 1)
        {
            _Pager = new PageContext(page_size, current_page);
            return this;
        }

        /// <summary>
        /// 为查询设置分页信息（SQL 2000 及 Microsoft Access分页兼容，其它数据库不必使用此方法）。
        /// </summary>
        /// <param name="page_size">每页的条目数量。</param>
        /// <param name="current_page">当前页（从第一页开始）。</param>
        /// <param name="helpField">用于辅助分页的字段（某些数据库需要字段来辅助分页,将使排序设置失效）。</param>
        /// <param name="helpSort">辅助分页字段的排序模式。</param>
        /// <returns></returns>
        public SelectBlock Paging(int page_size, int current_page, FieldDescription helpField, OrderByMode helpSort = OrderByMode.DESC)
        {
            _Pager = new PageContext(page_size, current_page, helpField, helpSort);
            return this;
        }
    }
}
