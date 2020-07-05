using System;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wunion.DataAdapter.Kernel;

namespace UnitTestProject
{
    [TestClass]
    public class DropTableUnitTest : WdaUnitTest
    {
        private void ExecuteDrop(BatchCommander batch, string tableName)
        {
            if (batch.TableExists(tableName))
            {
                try
                {
                    batch.DropTable(tableName);
                    Debug.WriteLine(string.Format("已成功删除表 {0} ！", tableName));
                }
                catch (Exception Ex)
                {
                    Debug.WriteLine(Ex.Message);
                }
            }
            else
            {
                Debug.WriteLine(string.Format("表 {0} 不存在，未执行删除操作！", tableName));
            }
        }

        private void ExecuteDrop(DataEngine db)
        {
            using (BatchCommander batch = new BatchCommander(db))
            {
                ExecuteDrop(batch, table_products);
                ExecuteDrop(batch, table_categories);
            }
        }

        [TestMethod]
        public void SQLServer()
        {
            Debug.WriteLine("");
            Debug.WriteLine("Microsoft SQL Server -----------------");
            DataEngine db = GetDbContext("ms-sql");
            ExecuteDrop(db);
            ReleaseDb(db);
        }

        [TestMethod]
        public void MySQL()
        {
            Debug.WriteLine("");
            Debug.WriteLine("MySQL -----------------");
            DataEngine db = GetDbContext("mysql");
            ExecuteDrop(db);
            ReleaseDb(db);
        }

        [TestMethod]
        public void PostgreSQL()
        {
            Debug.WriteLine("");
            Debug.WriteLine("PostgreSQL -----------------");
            DataEngine db = GetDbContext("npgsql");
            ExecuteDrop(db);
            ReleaseDb(db);
        }

        [TestMethod]
        public void SQLite3()
        {
            Debug.WriteLine("");
            Debug.WriteLine("SQLite3 -----------------");
            DataEngine db = GetDbContext("sqlite3", useConnectionPool: false);
            ExecuteDrop(db);
        }
    }
}
