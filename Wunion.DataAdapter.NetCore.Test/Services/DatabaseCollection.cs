using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.NetCore.Test
{
    /// <summary>
    /// 表示此应用程序的数据库集合.
    /// </summary>
    public class DatabaseCollection
    {
        /// <summary>
        /// 创建一个 <see cref="DatabaseCollection"/> 的对象实例.
        /// </summary>
        public DatabaseCollection()
        { }

        /// <summary>
        /// 表示 Microsoft SQL Server 数据库引擎.
        /// </summary>
        public DataEngine Mssql { get; private set; }

        /// <summary>
        /// 表示 MySQL 数据库引擎.
        /// </summary>
        public DataEngine MySql { get; private set; }

        /// <summary>
        /// 表示 PostgreSQL 数据库引擎.
        /// </summary>
        public DataEngine PostgreSQL { get; private set; }

        /// <summary>
        /// 获取当前正在使用中的数据库引擎对象.
        /// </summary>
        public DataEngine Current { get; private set; }

        /// <summary>
        /// 获取当前引擎的数据库类型.
        /// </summary>
        public string CurrentDbType { get; private set; }

        /// <summary>
        /// 表示 SQLite3 数据库引擎.
        /// </summary>
        public DataEngine SQLite3 { get; private set; }

        private void SetConnectionPool(DataEngine engine, string connectionString, int connectionPool)
        {
            if (connectionPool < 1)
                return;
            engine.UseDefaultConnectionPool((pool) => {
                pool.RequestTimeout = TimeSpan.FromSeconds(3);
                pool.ReleaseTimeout = TimeSpan.FromMinutes(5);
                pool.MaximumConnections = connectionPool;
            });
        }

        /// <summary>
        /// 配置 Microsoft SQL Server 数据库引擎.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        /// <param name="connectionPool">连接池数量（0 表示禁用）.</param>
        public void UseSqlserver(string connectionString, int connectionPool = 0)
        {
            DbAccess dba = new Wunion.DataAdapter.Kernel.SQLServer.SqlServerDbAccess();
            dba.ConnectionString = connectionString;
            ParserAdapter adapter = new Wunion.DataAdapter.Kernel.SQLServer.CommandParser.SqlServerParserAdapter();
            Mssql = new DataEngine(dba, adapter);
            SetConnectionPool(Mssql, connectionString, connectionPool);
        }

        /// <summary>
        /// 配置 MySQL 数据库引擎.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        /// <param name="connectionPool">连接池数量（0 表示禁用）.</param>
        public void UseMySql(string connectionString, int connectionPool = 0)
        {
            DbAccess dba = new Wunion.DataAdapter.Kernel.MySQL.MySqlDBAccess();
            dba.ConnectionString = connectionString;
            ParserAdapter adapter = new Wunion.DataAdapter.Kernel.MySQL.CommandParser.MySqlParserAdapter();
            MySql = new DataEngine(dba, adapter);
            SetConnectionPool(MySql, connectionString, connectionPool);
        }

        /// <summary>
        /// 配置 PostgreSQL 数据库引擎.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        /// <param name="connectionPool">连接池数量（0 表示禁用）.</param>
        public void UsePostgreSQL(string connectionString, int connectionPool = 0)
        {
            DbAccess dba = new Wunion.DataAdapter.Kernel.PostgreSQL.NpgsqlDbAccess();
            dba.ConnectionString = connectionString;
            ParserAdapter adapter = new Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser.NpgsqlParserAdapter();
            PostgreSQL = new DataEngine(dba, adapter);
            SetConnectionPool(PostgreSQL, connectionString, connectionPool);
        }

        /// <summary>
        /// 配置 SQLite3 数据库引擎.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public void UseSQLite3(string connectionString)
        {
            DbAccess dba = new Wunion.DataAdapter.Kernel.SQLite3.SqliteDbAccess();
            dba.ConnectionString = connectionString;
            ParserAdapter adapter = new Wunion.DataAdapter.Kernel.SQLite3.CommandParser.SqliteParserAdapter();
            SQLite3 = new DataEngine(dba, adapter);
        }

        /// <summary>
        /// 将给定类型的数据库设置为当前活动的数据库引擎.
        /// </summary>
        /// <param name="dbType">数据库类型的名称.</param>
        public void SetActive(string dbType)
        {
            if (string.IsNullOrEmpty(dbType))
                return;
            string typeName = dbType.ToLower();
            switch (typeName)
            {
                case "ms-sql":
                    Current = Mssql;
                    CurrentDbType = typeName;
                    break;
                case "mysql":
                    Current = MySql;
                    CurrentDbType = typeName;
                    break;
                case "npgsql":
                    Current = PostgreSQL;
                    CurrentDbType = typeName;
                    break;
                case "sqlite3":
                    Current = SQLite3;
                    CurrentDbType = typeName;
                    break;
                default:
                    throw new NotSupportedException(string.Format("Unsupported database: {0}\r\n不支持的数据库.", dbType));
            }
        }
    }
}
