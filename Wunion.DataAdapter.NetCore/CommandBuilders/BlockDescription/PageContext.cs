using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 内容分页信息对象类型。
    /// </summary>
    public class PageContext
    {
        private int _PageSize;
        private int _CurrentPage;
        private FieldDescription _HelpField;
        private OrderByMode _HelpSort;

        /// <summary>
        /// 实例化一个内容分页器。
        /// </summary>
        /// <param name="page_size">每页条目数量。</param>
        /// <param name="current_page">当前页。</param>
        public PageContext(int page_size, int current_page)
        {
            _PageSize = page_size;
            _CurrentPage = current_page;
        }

        /// <summary>
        /// 实例化一个内容分页器（SQL 2000 及 Microsoft Access分页兼容，其它数据库不必使用此方法）。
        /// </summary>
        /// <param name="page_size">每页条目数量。</param>
        /// <param name="current_page">当前页。</param>
        /// <param name="helpField">分页的辅助字段信息。</param>
        /// <param name="helpSort">分页辅助字段的排序模式。</param>
        public PageContext(int page_size, int current_page, FieldDescription helpField, OrderByMode helpSort)
        {
            _PageSize = page_size;
            _CurrentPage = current_page;
            _HelpField = helpField;
            _HelpSort = helpSort;
        }

        /// <summary>
        /// 获取分页时的每页条目数量。
        /// </summary>
        public int PageSize
        {
            get { return _PageSize; }
        }

        /// <summary>
        /// 获取当前页。
        /// </summary>
        public int CurrentPage
        {
            get { return _CurrentPage; }
        }

        /// <summary>
        /// 获取分页的辅助字段信息。
        /// </summary>
        public FieldDescription HelpField
        {
            get { return _HelpField; }
        }

        /// <summary>
        /// 获取分页辅助字段的排序模式。
        /// </summary>
        public OrderByMode HelpSort
        {
            get { return _HelpSort; }
        }
    }
}
