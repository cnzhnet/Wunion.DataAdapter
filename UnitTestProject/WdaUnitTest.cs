using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.SQLServer.CommandParser;
using Wunion.DataAdapter.Kernel.MySQL.CommandParser;
using Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandParser;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.Kernel.MySQL;
using Wunion.DataAdapter.Kernel.PostgreSQL;
using Wunion.DataAdapter.Kernel.SQLite3;
using Wunion.DataAdapter.Kernel.SQLite3.CommandParser;

namespace UnitTestProject
{
    public abstract class WdaUnitTest
    {
        public const string table_categories = "Categories";
        public const string table_products = "Products";

        protected WdaUnitTest()
        { }

        protected DataEngine GetDbContext(string dbtype, bool useConnectionPool = true)
        {
            DbAccess dba = null;
            ParserAdapter adapter = null;
            switch (dbtype?.ToLower())
            {
                case "ms-sql":
                    dba = new SqlServerDbAccess();
                    dba.ConnectionString = "Server=(local);Database=Wunion.DataAdapter.NetCore.Demo;User ID=sa;Password=ms-sql@(*~-^*);";
                    adapter = new SqlServerParserAdapter();
                    break;
                case "mysql":
                    dba = new MySqlDBAccess();
                    dba.ConnectionString = "Data Source=192.168.1.106;Database=ksdatab;User ID=cnzhnet;Password=mysql@(*~-^*);";
                    adapter = new MySqlParserAdapter();
                    break;
                case "npgsql":
                    dba = new NpgsqlDbAccess();
                    dba.ConnectionString = "Host=192.168.1.106;Username=postgres;Password=npgsql@(*~-^*);Database=Wunion.DataAdapter.NetCore.Demo;";
                    adapter = new NpgsqlParserAdapter();
                    break;
                case "sqlite3":
                    dba = new SqliteDbAccess();
                    dba.ConnectionString = @"Data Source=E:\SQLiteStudio\wdak.sqlite3;";
                    adapter = new SqliteParserAdapter();
                    break;
            }
            if (dba == null || adapter == null)
                return null;
            DataEngine db = new DataEngine(dba, adapter);
            if (useConnectionPool)
                UseConnectionPool(db);
            return db;
        }

        protected virtual void UseConnectionPool(DataEngine db)
        {
            db.UseDefaultConnectionPool((pool) => {
                pool.RequestTimeout = TimeSpan.FromSeconds(15);
                pool.ReleaseTimeout = TimeSpan.FromMinutes(10);
                pool.MaximumConnections = 32;
            });
        }

        protected void ReleaseDb(DataEngine db)
        {
            db.DBA.ConnectionPool.Dispose();
        }
    }

    public enum TestExecutePattern
    { 
        InBatchCommander = 0,
        InTransaction = 1
    }
}
