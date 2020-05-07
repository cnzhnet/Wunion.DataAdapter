using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.NetCore.Test.Models
{
    /// <summary>
    /// 表示分页的数据.
    /// </summary>
    /// <typeparam name="TEntity">数据集的元素类型名称.</typeparam>
    [Serializable()]
    public class PaginatedCollection<TEntity> where TEntity : class
    {
        /// <summary>
        /// 创建一个 <see cref="PaginatedCollection{TEntity}"/> 的对象实例.
        /// </summary>
        /// <param name="total">数据的总页数.</param>
        /// <param name="page">当前页.</param>
        /// <param name="dataItems">数据集合.</param>
        public PaginatedCollection(int total, int page, List<TEntity> dataItems) 
        {
            this.Total = total;
            this.Page = page;
            this.Items = dataItems;
        }

        /// <summary>
        /// 获取数据总条数.
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// 获取数据的页码.
        /// </summary>
        public int Page { get; private set; }

        /// <summary>
        /// 获取该页的集合.
        /// </summary>
        public List<TEntity> Items { get; private set; }
    }
}
