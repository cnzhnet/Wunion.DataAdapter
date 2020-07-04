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
    public class TableCreateUnitTest
    {
        [TestMethod]
        public void CreateTable()
        {
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.CreateTable("MyTable1").ColumnsDefine(
                DbTableColumnDefinition.New(name: "Id", dataType: GenericDbType.Int, notNull: true, identity: new DbColumnIdentity(0, 1), pk: true),
                DbTableColumnDefinition.New(name: "Category", dataType: GenericDbType.Int, notNull: true),
                DbTableColumnDefinition.New(name: "Name", dataType: GenericDbType.VarChar, size: 32, notNull: true, unique: true),
                DbTableColumnDefinition.New(name: "Price", dataType: GenericDbType.Single, Default: 0f),
                DbTableColumnDefinition.New(name: "Quantity", dataType: GenericDbType.SmallInt, Default: 0),
                DbTableColumnDefinition.New(name: "Remark", dataType: GenericDbType.VarChar, size: 300),
                DbTableColumnDefinition.New(name: "TimeOf", dataType: GenericDbType.DateTime, Default: Fun.Now()),
                DbTableColumnDefinition.New(name: "Picture", dataType: GenericDbType.Image)
            );
            StringBuilder buffers = new StringBuilder();
            buffers.Append("-- SQL Server ----------------------").AppendLine();
            buffers.Append(cb.Parsing(new SqlServerParserAdapter()));
            buffers.AppendLine().Append("-- MySQL ----------------------");
            cb.Clean();
            buffers.AppendLine().Append(cb.Parsing(new MySqlParserAdapter()));          
            cb.Clean();
            buffers.AppendLine().Append("-- PostgreSQL ----------------------");
            buffers.AppendLine().Append(cb.Parsing(new NpgsqlParserAdapter()));
            string sql = buffers.ToString();
            Debug.Write(sql);
        }
    }
}
