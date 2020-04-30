using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.MySQL;
using Wunion.DataAdapter.EntityUtils;
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Services
{
    public class MySQLDbContext : IDatabaseContext
    {
        /// <summary>
        /// 获取或设置数据引擎.
        /// </summary>
        public DataEngine DbEngine { get; }

        /// <summary>
        /// 创建一个 <see cref="MySQLDbContext"/> 的对象实例.
        /// </summary>
        /// <param name="engine"></param>
        public MySQLDbContext(DataEngine engine)
        {
            DbEngine = engine;
        }

        /// <summary>
        /// 获取数据库表的信息.
        /// </summary>
        /// <returns></returns>
        public List<TableInfoModel> GetTables()
        {
            List<TableInfoModel> Result = null;
            using (MySqlCommand Command = (MySqlCommand)DbEngine.DBA.CreateDbCommand())
            {
                Command.Connection = (MySqlConnection)DbEngine.DBA.GetConnection();
                Command.CommandText = global::Wunion.DataAdapter.EntityGenerator.Properties.Resources.MySQLDbSchema;
                Command.CommandType = CommandType.Text;
                Command.Parameters.Add((MySqlParameter)(DbEngine.DBA.CreateParameter("dbName", GetDbName())));
                Command.Connection.Open();
                MySqlDataReader Rd = Command.ExecuteReader(CommandBehavior.CloseConnection);
                Result = new List<TableInfoModel>();
                int i;
                TableInfoModel tableInfo;
                while (Rd.Read())
                {
                    tableInfo = new TableInfoModel();
                    for (i = 0; i < Rd.FieldCount; ++i)
                        tableInfo.SetValue(Rd.GetName(i), Rd.GetValue(i), true);
                    Result.Add(tableInfo);
                }
                Rd.Close();
            }
            return Result;
        }

        /// <summary>
        /// 获取数据库名称.
        /// </summary>
        /// <returns></returns>
        private string GetDbName()
        {
            string[] connOptions = DbEngine.DBA.ConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] item;
            foreach (string s in connOptions)
            {
                item = s.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (item.Length != 2)
                    continue;
                if (item[0].ToLower().Trim() == "database")
                    return item[1].Trim();
            }
            return null;
        }
    }
}
