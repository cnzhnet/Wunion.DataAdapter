using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandParser;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.Kernel.SQLServer.CommandParser;
using Wunion.DataAdapter.EntityUtils.CodeProvider;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.CommandProviders
{
    /// <summary>
    /// Connect 命令的支持.
    /// </summary>
    public static class ConnectCommandProvider
    {
        /// <summary>
        /// 执行 Connect 命令.
        /// </summary>
        /// <param name="parameters">命令的参数列表.</param>
        /// <param name="lang">命令输出的语言环境服务.</param>
        /// <returns></returns>
        public static GeneratorService Do(List<string> parameters, LanguageService lang)
        {
            if (parameters == null || parameters.Count < 2)
            {
                WriteInstructions();
                return null;
            }
            string connectionString = ParametersPathGetter.MergeOne(parameters, 1);
            GeneratorService service = null;
            DbAccess DBA;
            switch (parameters[0].ToLower())
            {
                case "ms-sql":
                    DBA = new Wunion.DataAdapter.Kernel.SQLServer.SqlServerDbAccess();
                    DBA.ConnectionString = connectionString;
                    service = new GeneratorService(DBA, new Wunion.DataAdapter.Kernel.SQLServer.CommandParser.SqlServerParserAdapter());
                    service.DbContext = new SqlServerDbContext(service.DbEngine);
                    break;
                case "sqlite":
                    DBA = new Wunion.DataAdapter.Kernel.SQLite3.SqliteDbAccess();
                    DBA.ConnectionString = connectionString;
                    service = new GeneratorService(DBA, new Wunion.DataAdapter.Kernel.SQLite3.CommandParser.SqliteParserAdapter());
                    service.DbContext = new SQLite3DbContext(service.DbEngine);
                    break;
                case "postgresql":
                    DBA = new Wunion.DataAdapter.Kernel.PostgreSQL.NpgsqlDbAccess();
                    DBA.ConnectionString = connectionString;
                    service = new GeneratorService(DBA, new Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser.NpgsqlParserAdapter());
                    service.DbContext = new PostgreSQLDbContext(service.DbEngine);
                    break;
                case "mysql":
                    DBA = new Wunion.DataAdapter.Kernel.MySQL.MySqlDBAccess();
                    DBA.ConnectionString = connectionString;
                    service = new GeneratorService(DBA, new Wunion.DataAdapter.Kernel.MySQL.CommandParser.MySqlParserAdapter());
                    service.DbContext = new MySQLDbContext(service.DbEngine);
                    break;
            }
            if (service != null)
            {
                try
                {
                    List<TableInfoModel> lst = service.AllTables.Distinct(p => p.tableName).ToList();
                    Console.WriteLine(lang.GetString("DatabaseConnected"));
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                }
            }
            return service;
        }

        /// <summary>
        /// 输出命令的使用方法 .
        /// </summary>
        public static void WriteInstructions()
        {
            Console.WriteLine(string.Format("\tconnect <ms-sql | sqlite | PostgreSQL | mysql> <ConnectionString>"));
        }
    }
}
