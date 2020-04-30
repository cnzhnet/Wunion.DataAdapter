using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Data.SqlClient;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Services
{
    public class SqlServerDbContext : IDatabaseContext
    {
        /// <summary>
        /// 获取或设置数据引擎.
        /// </summary>
        public DataEngine DbEngine { get; }

        public SqlServerDbContext(DataEngine engine)
        {
            DbEngine = engine;
        }

        /// <summary>
        /// 获取数据库表的信息.
        /// </summary>
        /// <returns></returns>
        public List<TableInfoModel> GetTables()
        {
            string commandText = global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.SQLServerDbSchema;
            //command = command.Replace("{TABLE_NAME}", TableName);
            List <TableInfoModel> Result = null;
            using (SqlCommand Command = (SqlCommand)DbEngine.DBA.CreateDbCommand())
            {
                Command.Connection = (SqlConnection)DbEngine.DBA.Connect();
                Command.CommandText = commandText;
                Command.CommandType = CommandType.Text;
                Command.Connection.Open();
                SqlDataReader Rd = Command.ExecuteReader(CommandBehavior.CloseConnection);

                Result = new List<TableInfoModel>();
                int i;
                string field;
                TableInfoModel tableInfo;
                while (Rd.Read())
                {
                    tableInfo = new TableInfoModel();
                    for (i = 0; i < Rd.FieldCount; ++i)
                    {
                        field = Rd.GetName(i);
                        tableInfo.SetValue(field, Rd.GetValue(i), true);
                    }
                    Result.Add(tableInfo);
                }
                Rd.Close();
            }
            return Result;
        }
    }
}
