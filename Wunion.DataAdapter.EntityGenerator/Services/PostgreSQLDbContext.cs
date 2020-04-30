using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.ComponentModel;
using Npgsql;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.MySQL;
using Wunion.DataAdapter.EntityUtils;
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Services
{
    public class PostgreSQLDbContext : IDatabaseContext
    {
        /// <summary>
        /// 获取或设置数据引擎.
        /// </summary>
        public DataEngine DbEngine { get; }

        /// <summary>
        /// 创建一个 <see cref="PostgreSQLDbContext"/> 的对象实例.
        /// </summary>
        /// <returns></returns>
        public PostgreSQLDbContext(DataEngine engine)
        {
            DbEngine = engine;
        }

        /// <summary>
        /// 获取数据库表的信息.
        /// </summary>
        /// <returns></returns>
        public List<TableInfoModel> GetTables()
        {
            string commandText = global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.PostgreSQLDbSchema;
            List<TableInfoModel> Result = null;
            using (NpgsqlCommand Command = (NpgsqlCommand)DbEngine.DBA.CreateDbCommand())
            {
                Command.Connection = (NpgsqlConnection)DbEngine.DBA.Connect();
                Command.CommandText = commandText;
                Command.CommandType = CommandType.Text;
                Command.Connection.Open();
                NpgsqlDataReader Rd = Command.ExecuteReader(CommandBehavior.CloseConnection);

                Result = new List<TableInfoModel>();
                int i;
                TableInfoModel tableInfo;
                while (Rd.Read())
                {
                    tableInfo = new TableInfoModel();
                    tableInfo.SetValue("tableName", Rd["tablename"], true);
                    tableInfo.SetValue("paramName", Rd["paramname"], true);
                    tableInfo.SetValue("allowNull", Rd["allownull"], true);
                    tableInfo.SetValue("dbType", Rd["dbtype"], true);
                    tableInfo.SetValue("isPrimary", Rd["isprimary"], true);
                    tableInfo.SetValue("isIdentity", Rd["isidentity"], true);
                    Result.Add(tableInfo);
                }
                Rd.Close();
            }
            return Result;
        }
    }
}
