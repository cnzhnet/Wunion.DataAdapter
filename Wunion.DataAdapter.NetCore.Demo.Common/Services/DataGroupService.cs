using System;
using System.Collections.Generic;
using System.Linq;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.DataCollection;

namespace Wunion.DataAdapter.NetCore.Demo.Services
{
    public class DataGroupService
    {
        private const string TableName = "T_TEST1";
        public bool AddGroup(string Name)
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Insert(fm.Table(TableName), td.Field("GroupName"))
                   .Values(Name);
            int result = DataEngine.CurrentEngine.DBA.ExecuteNoneQuery(Command);
            return result > 0;
        }

        public SpeedDataTable GetAllGroup()
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Select(td.Field("*")).From(TableName).OrderBy(td.Field("GroupId"), OrderByMode.ASC);
            return DataEngine.CurrentEngine.ExecuteQuery(Command);
        }
    }
}
