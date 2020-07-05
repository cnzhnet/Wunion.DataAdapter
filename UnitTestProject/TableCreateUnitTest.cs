using System;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.DbInterop;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class TableCreateUnitTest : WdaUnitTest
    {
        private DbCommandBuilder categoriesBuilder;
        private DbCommandBuilder productsBuilder;

        public TableCreateUnitTest()
        {
            // create table 命令构建.
            categoriesBuilder = new DbCommandBuilder();
            categoriesBuilder.CreateTable(table_categories).ColumnsDefine(
                DbTableColumnDefinition.New(name: "Id", dataType: GenericDbType.Int, notNull: true, identity: new DbColumnIdentity(0, 1), pk: true),
                DbTableColumnDefinition.New(name: "CategoryName", dataType: GenericDbType.VarChar, size: 255, notNull: true, unique: true),
                DbTableColumnDefinition.New(name: "Remark", dataType: GenericDbType.VarChar, size: 300),
                DbTableColumnDefinition.New(name: "Cover", dataType: GenericDbType.Image),
                DbTableColumnDefinition.New(name: "CreationDate", dataType: GenericDbType.DateTime, notNull: true, Default: Fun.Now())
            );

            productsBuilder = new DbCommandBuilder();
            productsBuilder.CreateTable(table_products).ColumnsDefine(
                DbTableColumnDefinition.New(name: "Id", dataType: GenericDbType.Int, notNull: true, identity: new DbColumnIdentity(0, 1), pk: true),
                DbTableColumnDefinition.New(name: "Category", dataType: GenericDbType.Int, notNull: true, fk: new DbForeignKey(table_categories, "Id")),
                DbTableColumnDefinition.New(name: "Name", dataType: GenericDbType.VarChar, size: 32, notNull: true, unique: true),
                DbTableColumnDefinition.New(name: "Price", dataType: GenericDbType.Single, Default: 0f),
                DbTableColumnDefinition.New(name: "Quantity", dataType: GenericDbType.SmallInt, Default: 0),
                DbTableColumnDefinition.New(name: "Remark", dataType: GenericDbType.VarChar, size: 300),
                DbTableColumnDefinition.New(name: "TimeOf", dataType: GenericDbType.DateTime, Default: Fun.Now()),
                DbTableColumnDefinition.New(name: "Picture", dataType: GenericDbType.Image)
            );
        }

        private DbCommandBuilder GetTableBuilder(string tableName)
        {
            if (tableName == table_categories)
                return categoriesBuilder;
            else
                return productsBuilder;
        }

        private bool ExecuteCreateTable(TestExecutePattern pattern, DataEngine db, string tableName, object controller)
        {
            int result = 0;
            DbCommandBuilder cb = GetTableBuilder(tableName);
            Debug.WriteLine("");
            if (pattern == TestExecutePattern.InTransaction)
            {
                Debug.WriteLine("在事务中执行！");
                DBTransactionController trans = (DBTransactionController)controller;
                Debug.WriteLine(cb.Parsing(db.CommandParserAdapter));
                Debug.WriteLine("执行结果：");
                if (trans.DBA.TableExists(tableName))
                {
                    Debug.Write(string.Format("创建失败，表名 {0} 在数据库中已存在!", tableName));
                    return false;
                }
                else
                {
                    result = trans.DBA.ExecuteNoneQuery(cb); // execute create table.                        
                    if (result != -1)
                    {
                        Debug.Write(string.Format("成功创建表 {0} ！", tableName));
                        return true;
                    }
                    else
                    {
                        Debug.Write(trans.DBA.Errors.LastOrDefault().Message);
                        return false;
                    }
                }
            }
            else
            {
                Debug.WriteLine("批量处理器执行！");
                BatchCommander batch = (BatchCommander)controller;
                Debug.WriteLine(cb.Parsing(db.CommandParserAdapter));
                Debug.WriteLine("执行结果：");
                if (batch.TableExists(tableName))
                {
                    Debug.Write(string.Format("表名 {0} 在数据库中已存在!", tableName));
                    return false;
                }
                else
                {
                    result = batch.ExecuteNonQuery(cb); // execute create table.                        
                    if (result != -1)
                    {
                        Debug.Write(string.Format("成功创建表 {0} ！", tableName));
                        return true;
                    }
                    else
                    {
                        Debug.Write(string.Format("表 {0} 创建失败！", tableName));
                        return false;
                    }
                }
            }
        }

        [TestMethod]
        public void SQLServer()
        {
            Debug.WriteLine("");
            Debug.WriteLine("Microsoft SQL Server -----------------");
            DataEngine db = GetDbContext("ms-sql");
            using (DBTransactionController trans = db.BeginTrans())
            {
                bool success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_categories, trans);
                if (success)
                 success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_products, trans);
                if (success)
                    trans.Commit();
            }
            ReleaseDb(db);
        }

        [TestMethod]
        public void MySQL()
        {
            Debug.WriteLine("");
            Debug.WriteLine("MySQL -----------------");
            DataEngine db = GetDbContext("mysql");
            using (DBTransactionController trans = db.BeginTrans())
            {
                bool success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_categories, trans);
                if (success)
                    success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_products, trans);
                if (success)
                    trans.Commit();
            }
            ReleaseDb(db);
        }

        [TestMethod]
        public void PostgreSQL()
        {
            Debug.WriteLine("");
            Debug.WriteLine("PostgreSQL -----------------");
            DataEngine db = GetDbContext("npgsql");
            using (DBTransactionController trans = db.BeginTrans())
            {
                bool success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_categories, trans);
                if (success)
                    success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_products, trans);
                if (success)
                    trans.Commit();
            }
            ReleaseDb(db);
        }

        [TestMethod]
        public void SQLite3()
        {
            Debug.WriteLine("");
            Debug.WriteLine("SQLite3 -----------------");
            DataEngine db = GetDbContext("sqlite3", useConnectionPool: false);
            using (DBTransactionController trans = db.BeginTrans())
            {
                bool success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_categories, trans);
                if (success)
                    success = ExecuteCreateTable(TestExecutePattern.InTransaction, db, table_products, trans);
                if (success)
                    trans.Commit();
            }
        }
    }
}
