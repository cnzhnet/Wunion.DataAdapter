using System;
using System.Collections.Generic;
using System.Linq;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.NetCore.Test.Models;

namespace Wunion.DataAdapter.NetCore.Test.Services
{
    /// <summary>
    /// 测试分组数据的服务.
    /// </summary>
    public class GroupDataService : DataService
    {
        /// <summary>
        /// 分组表名称.
        /// </summary>
        public string tableName => "GroupTable";

        /// <summary>
        /// 数据表名称.
        /// </summary>
        public string ItemsTable => "TestDataItems";

        /// <summary>
        /// 创建一个 <see cref="GroupDataService"/> 的对象实例.
        /// </summary>
        public GroupDataService() { }

        /// <summary>
        /// 查询所有分组数据.
        /// </summary>
        /// <returns></returns>
        public List<dynamic> Query()
        {
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.Select(td.Field("GroupId"), td.Field("GroupName")).From(tableName).OrderBy(td.Field("GroupId"), OrderByMode.DESC);
            return db.ExecuteDynamicEntity(cb);
        }
    }
}
