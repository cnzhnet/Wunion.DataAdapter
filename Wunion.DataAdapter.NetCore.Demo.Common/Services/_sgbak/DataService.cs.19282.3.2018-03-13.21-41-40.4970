using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.NetCore.Demo.Services
{
    public class DataService
    {
        private const string GRP_TABLE_NAME = "T_TEST1";
        private const string TABLE_NAME = "T_TEST2";

        protected bool AddData(Dictionary<string, object> data, DBTransactionController trans = null)
        {
            if (data == null)
                return false;
            if (!data.ContainsKey("TestPhoto"))
                data.Add("TestPhoto", DBNull.Value);
            QuickDataChanger QuickChange = new QuickDataChanger(trans, DataEngine.CurrentEngine);
            int result = QuickChange.SaveToDataBase(TABLE_NAME, data, false);
            return result > 0;
        }

        protected bool UpdateData(Dictionary<string, object> data, DBTransactionController trans = null)
        {
            QuickDataChanger QuickChange = new QuickDataChanger(trans, DataEngine.CurrentEngine);
            QuickChange.Conditions.Add(td.Field("TestId") == data["TestId"]);
            data.Remove("TestId"); // TestId 传进来作为条件，不更新它（所以这里要从字典中删除）。
            int result = QuickChange.SaveToDataBase(TABLE_NAME, data, true);
            return result > 0;
        }

        public bool SaveData(Dictionary<string, object> data)
        {
            bool ok = false;
            using (DBTransactionController trans = DataEngine.CurrentEngine.BeginTrans())
            {
                if (data.ContainsKey("TestId"))
                    ok = UpdateData(data, trans);
                else
                    ok = AddData(data, trans);
                trans.Commit();
            }
            return ok;
        }

        /// <summary>
        /// 获取页数。
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int GetPageCount(int groupId, int pageSize)
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Select(Fun.Count("*")).From(fm.Table(TABLE_NAME)).Where(td.Field("GroupId") == groupId);
            object result = DataEngine.CurrentEngine.ExecuteScalar(Command);
            float count = Convert.ToSingle(GetTotalCount(groupId));
            if ((count % pageSize) == 0)
                return (int)(count / pageSize);
            else
                return (int)(count / pageSize + 1);
        }

        /// <summary>
        /// 获取总记录数量。
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public int GetTotalCount(int groupId)
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Select(Fun.Count("*")).From(fm.Table(TABLE_NAME)).Where(td.Field("GroupId") == groupId);
            object result = DataEngine.CurrentEngine.ExecuteScalar(Command);
            return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
        }

        public SpeedDataTable GetGroupData(int groupId, int pageSize, int currentPage)
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Select(td.Field("t2", "TestId"),
                           td.Field("t1", "GroupName"),
                           td.Field("t2", "TestName"),
                           td.Field("t2", "TestAge"),
                           td.Field("t2", "TestSex"))
                   .From(fm.Table(TABLE_NAME, "t2"),
                         fm.LeftJoin(GRP_TABLE_NAME, "t1")
                           .ON(td.Field("t2", "GroupId") == td.Field("t1", "GroupId"))
                   )
                   .Where(td.Field("t2", "GroupId") == groupId)
                   .OrderBy(td.Field("t2", "TestId"), OrderByMode.DESC)
                   .Paging(pageSize, currentPage);
            return DataEngine.CurrentEngine.ExecuteQuery(Command);
        }

        public Dictionary<string, object> GetDetails(int dataId)
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Select(td.Field("*")).From(fm.Table(TABLE_NAME)).Where(td.Field("TestId") == dataId);
            IDataReader Rd = DataEngine.CurrentEngine.ExecuteReader(Command);
            if (Rd == null)
                return null;
            Dictionary<string, object> data = new Dictionary<string, object>();
            while (Rd.Read())
            {
                data.Add("data_id", Rd["TestId"]);
                data.Add("group_id", Rd["GroupId"]);
                data.Add("name", Rd["TestName"]);
                data.Add("age", Rd["TestAge"]);
                data.Add("sex", Rd["TestSex"]);
                //data.Add("photo", Rd["TestPhoto"]);
            }
            Rd.Close();
            Rd.Dispose();
            return data;
        }

        /// <summary>
        /// 获取照片数据。
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public byte[] GetPhotoData(int dataId)
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Select(td.Field("TestPhoto")).From(TABLE_NAME).Where(td.Field("TestId") == dataId);
            object Data = DataEngine.CurrentEngine.ExecuteScalar(Command);
            if (Data == null || Data == DBNull.Value)
                return null;
            return (byte[])Data;
        }

        public bool DeleteData(object[] filters)
        {
            int result = -1;
            using (DBTransactionController Trans = DataEngine.CurrentEngine.BeginTrans())
            {
                DbCommandBuilder Command = new DbCommandBuilder();
                Command.Delete(fm.Table(TABLE_NAME)).Where(td.Field("TestId").In(filters));
                result = Trans.DBA.ExecuteNoneQuery(Command);
                Trans.Commit();
            }
            return result > 0;
        }
    }
}
