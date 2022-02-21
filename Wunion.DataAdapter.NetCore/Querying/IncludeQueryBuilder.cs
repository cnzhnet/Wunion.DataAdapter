using System;
using System.Collections.Generic;
using System.Linq;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CodeFirst;

namespace Wunion.DataAdapter.Kernel.Querying
{
    /// <summary>
    /// 用于辅助构建联合查询.
    /// </summary>
    /// <typeparam name="TDAO"></typeparam>
    public class IncludeQueryBuilder<TDAO> where TDAO : QueryDao, new()
    {
        private Dictionary<Type, QueryDao> queryDaos;

        /// <summary>
        /// 创建一个 <see cref="IncludeQueryBuilder{TDAO}"/> 的对象实例.
        /// </summary>
        /// <param name="daos">已加入联合查询的实体数据访问器.</param>
        internal IncludeQueryBuilder(Dictionary<Type, QueryDao> daos)
        {
            queryDaos = daos;
        }

        /// <summary>
        /// 联合查询中的第一个表.
        /// </summary>
        public TDAO First => tbl<TDAO>();

        /// <summary>
        /// 获取联合查询中指定数据访问器对应的表.
        /// </summary>
        /// <typeparam name="TargetDao"></typeparam>
        /// <returns></returns>
        public TargetDao tbl<TargetDao>() where TargetDao : QueryDao
        {
            Type t = typeof(TargetDao);
            QueryDao targetDao = null;
            if (!queryDaos.TryGetValue(t, out targetDao))
                throw new Exception($"{t.Name} is not included in the query.");
            return (TargetDao)targetDao;
        }
    }
}
