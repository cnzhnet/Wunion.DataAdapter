using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.NetCore.Test.Models;

namespace Wunion.DataAdapter.NetCore.Test.Services
{
    /// <summary>
    /// 测试数据服务.
    /// </summary>
    public class TestDataItemService : DataService
    {
        private const string GROUP_TABLE = "GroupTable";

        /// <summary>
        /// 数据表名称.
        /// </summary>
        public string ItemsTable => "TestDataItems";

        /// <summary>
        /// 创建一个 <see cref="TestDataItemService"/> 的对象实例.
        /// </summary>
        public TestDataItemService() { }

        /// <summary>
        /// 查询测试数据.
        /// </summary>
        /// <param name="page">当前的页.</param>
        /// <param name="pageSize">每页数据条数.</param>
        /// <param name="group">分组ID（为 null 时查所有数据）.</param>
        /// <returns></returns>
        public PaginatedCollection<dynamic> Query(int page, int pageSize, int? group)
        {
            DbCommandBuilder cb = new DbCommandBuilder();
            SelectBlock sb = cb.Select(Fun.Count("*")).From(
                                       fm.Table(ItemsTable, "itm"),
                                       fm.LeftJoin(GROUP_TABLE, "grp").ON(td.Field("itm", "GroupId") == td.Field("grp", "GroupId"))
                               );
            List<object> condition = new List<object>();
            if (group != null)
            {
                condition.Add(td.Field("itm", "GroupId") == group.Value);
                sb.Where(condition.ToArray());
            }
            object count = db.ExecuteScalar(cb);
            int total = (count == null || count == DBNull.Value) ? 0 : Convert.ToInt32(count);
            if (total < 1)
                return null;
            sb = cb.Select(td.Field("itm", "TestId"), 
                           td.Field("grp", "GroupName"), 
                           td.Field("itm", "TestName"), 
                           td.Field("itm", "TestAge"), 
                           td.Field("itm", "TestSex")
                   ).From(
                          fm.Table(ItemsTable, "itm"), 
                          fm.LeftJoin(GROUP_TABLE, "grp").ON(td.Field("itm", "GroupId") == td.Field("grp", "GroupId"))
                   );
            if (condition.Count > 0)
                sb.Where(condition.ToArray());
            sb.OrderBy(td.Field("itm", "TestId"), OrderByMode.DESC).Paging(pageSize, page);
            List<dynamic> entities = db.ExecuteDynamicEntity(cb);
            return new PaginatedCollection<dynamic>(total, page, entities);
        }

        /// <summary>
        /// 搜索测试数据.
        /// </summary>
        /// <param name="keywords">关键字.</param>
        /// <param name="page">当前页.</param>
        /// <param name="pageSize">每页数据条数.</param>
        /// <param name="group">分组ID（为 null 时搜索所有数据）.</param>
        /// <returns></returns>
        public PaginatedCollection<dynamic> Search(string keywords, int page, int pageSize, int? group = null)
        {
            DbCommandBuilder cb = new DbCommandBuilder();
            object[] condition;
            if (group == null)
            {
                condition = new object[] { td.Field("itm", "TestName").Like(keywords, LikeMatch.Center) | td.Field("itm", "TestSex").Like(keywords, LikeMatch.Center) };
            }
            else
            {
                condition = new object[] { 
                    td.Field("itm", "GroupId") == group.Value & 
                    (td.Field("itm", "TestName").Like(keywords, LikeMatch.Center) | td.Field("itm", "TestSex").Like(keywords, LikeMatch.Center)).Group 
                };

            }
            cb.Select(Fun.Count("*"))
              .From(
                    fm.Table(ItemsTable, "itm"),
                    fm.LeftJoin(GROUP_TABLE, "grp").ON(td.Field("itm", "GroupId") == td.Field("grp", "GroupId"))
              ).Where(condition);
            object count = db.ExecuteScalar(cb);
            int total = (count == null || count == DBNull.Value) ? 0 : Convert.ToInt32(count);
            if (total < 1)
                return null;
            cb.Select(td.Field("itm", "TestId"),
                      td.Field("grp", "GroupName"),
                      td.Field("itm", "TestName"),
                      td.Field("itm", "TestAge"),
                      td.Field("itm", "TestSex")
              ).From(
                     fm.Table(ItemsTable, "itm"),
                     fm.LeftJoin(GROUP_TABLE, "grp").ON(td.Field("itm", "GroupId") == td.Field("grp", "GroupId"))
              ).Where(condition).OrderBy(td.Field("itm", "TestId"), OrderByMode.DESC).Paging(pageSize, page);
            List<dynamic> entities = db.ExecuteDynamicEntity(cb);
            return new PaginatedCollection<dynamic>(total, page, entities);
        }

        /// <summary>
        /// 获取指定记录的图片数据.
        /// </summary>
        /// <param name="dataId">记录ID.</param>
        /// <returns></returns>
        public byte[] GetPictureData(int dataId)
        {
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.Select(td.Field("TestPhoto")).From(ItemsTable).Where(td.Field("TestId") == dataId);
            byte[] result = db.ExecuteScalar(cb) as byte[];
            return (result == null) ? new byte[0] : result;
        }
    }
}
