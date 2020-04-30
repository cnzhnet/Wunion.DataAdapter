using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.ComponentModel;
using Microsoft.Data.Sqlite;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.EntityUtils;
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Services
{
    public class SQLite3DbContext : IDatabaseContext
    {
        /// <summary>
        /// 获取或设置数据引擎.
        /// </summary>
        public DataEngine DbEngine { get; }

        public SQLite3DbContext(DataEngine engine)
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
            using (SqliteCommand Command = (SqliteCommand)DbEngine.DBA.CreateDbCommand())
            {
                Command.Connection = (SqliteConnection)DbEngine.DBA.Connect();
                List<dynamic> tables = GetTableNames(Command);
                Result = new List<TableInfoModel>();
                for (int i = 0; i < tables.Count; ++i)
                {
                    if (tables[i].tbl_name == "sqlite_sequence")
                        continue;
                    GetTableInfo(Command, tables[i], Result);
                }
                if (Command.Connection.State != ConnectionState.Closed)
                    Command.Connection.Close();
            }
            return Result;
        }

        /// <summary>
        /// 获取指定表的信息.
        /// </summary>
        /// <param name="Command">用于执行命令的对象.</param>
        /// <param name="tbl">表名.</param>
        /// <param name="list">将表的列信息输出到此集合.</param>
        private void GetTableInfo(SqliteCommand Command, dynamic tbl, List<TableInfoModel> list)
        {
            Command.CommandText = string.Format("PRAGMA table_info([{0}])", tbl.tbl_name);
            if (Command.Connection.State != ConnectionState.Open)
                Command.Connection.Open();
            SqliteDataReader Rd = Command.ExecuteReader();
            TableInfoModel tableInfo;
            while (Rd.Read())
            {
                tableInfo = new TableInfoModel();
                tableInfo.tableName = tbl.tbl_name;
                tableInfo.paramName = Rd["name"] as string;
                tableInfo.dbType = FormatDbType(Rd["type"] as string);
                tableInfo.allowNull = !Convert.ToBoolean(Rd["notnull"]);
                tableInfo.defaultValue = (Rd["dflt_value"] == null || Rd["dflt_value"] == DBNull.Value) ? null : Rd["dflt_value"];
                tableInfo.isPrimary = Convert.ToBoolean(Rd["pk"]);
                tableInfo.isIdentity = IsIdentity(tableInfo.paramName, tbl.sql);
                list.Add(tableInfo);
            }
            Rd.Close();
        }

        /// <summary>
        /// 格式化输出的 SQLite 数据类型文本（主要用于去除长度信息）.
        /// </summary>
        /// <param name="dbType">数据库返回的数据类型字符串.</param>
        /// <returns></returns>
        private string FormatDbType(string dbType)
        {
            if (string.IsNullOrEmpty(dbType))
                return string.Empty;
            Regex Reg = new Regex(@"\(\s*\d+(\s*\,\s*\d+)*\s*\)");
            Match m = Reg.Match(dbType);
            if (m == null)
                return dbType;
            if (string.IsNullOrEmpty(m.Value))
                return dbType.ToLower();
            return dbType.Replace(m.Value, string.Empty).ToLower();
        }

        /// <summary>
        /// 判断是结给定的字段是否是自增长字段.
        /// </summary>
        /// <param name="name">字段名.</param>
        /// <param name="tbl_sql">表的 sql 命令.</param>
        /// <returns></returns>
        private bool IsIdentity(string name, string tbl_sql)
        {
            tbl_sql = tbl_sql.ToUpper();
            int index = tbl_sql.IndexOf(name.ToUpper());
            tbl_sql = tbl_sql.Substring(index);
            index = tbl_sql.IndexOf(",");
            if (index != -1)
                tbl_sql = tbl_sql.Substring(0, index);
            return tbl_sql.IndexOf("AUTOINCREMENT") != -1;
        }

        /// <summary>
        /// 获取所有表.
        /// </summary>
        /// <returns></returns>
        private List<dynamic> GetTableNames(SqliteCommand Command)
        {
            string sql = "SELECT tbl_name, sql FROM [SQLITE_MASTER] WHERE [type] = 'table' and [sql] <> ''";
            Command.CommandText = sql;
            Command.CommandType = CommandType.Text;
            Command.Connection.Open();
            SqliteDataReader Rd = Command.ExecuteReader();
            List<dynamic> tables = new List<dynamic>();
            DynamicEntity entity;
            int i;
            while (Rd.Read())
            {
                entity = new DynamicEntity();
                for (i = 0; i < Rd.FieldCount; ++i)
                    entity.SetPropertyValue(Rd.GetName(i), Rd.GetValue(i), Rd.GetFieldType(i));
                tables.Add(entity);
            }
            Rd.Close();
            return tables;
        }
    }
}
