using System;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.SQLServer.CommandParser;
using Wunion.DataAdapter.Kernel.MySQL.CommandParser;
using Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser;

namespace UnitTestProject
{
    [TestClass]
    public class TableExistsUnitTest : WdaUnitTest
    {
        [TestMethod]
        public void SQLServer()
        {
            Debug.WriteLine("");
            Debug.WriteLine("Microsoft SQL Server -----------------");
            DataEngine db = GetDbContext("ms-sql");
            if (db.DBA.TableExists("MyTable1"))
                Debug.WriteLine("数据库中存在表：MyTable1 .");
            else
                Debug.WriteLine("表：MyTable1 在数据库中并不存在.");
        }

        [TestMethod]
        public void MySQL()
        {
            Debug.WriteLine("");
            Debug.WriteLine("MySQL -----------------");
            DataEngine db = GetDbContext("mysql");
            if (db.DBA.TableExists("MyTable1"))
                Debug.WriteLine("数据库中存在表：MyTable1 .");
            else
                Debug.WriteLine("表：MyTable1 在数据库中并不存在.");
        }

        [TestMethod]
        public void PostgreSQL()
        {
            Debug.WriteLine("");
            Debug.WriteLine("PostgreSQL -----------------");
            DataEngine db = GetDbContext("npgsql");
            if (db.DBA.TableExists("MyTable1"))
                Debug.WriteLine("数据库中存在表：MyTable1 .");
            else
                Debug.WriteLine("表：MyTable1 在数据库中并不存在.");
        }

        [TestMethod]
        public void SQLite3()
        {
            Debug.WriteLine("");
            Debug.WriteLine("SQLite3 -----------------");
            DataEngine db = GetDbContext("sqlite3", useConnectionPool: false);
            if (db.DBA.TableExists("MyTable1"))
                Debug.WriteLine("数据库中存在表：MyTable1 .");
            else
                Debug.WriteLine("表：MyTable1 在数据库中并不存在.");
        }
    }
}
