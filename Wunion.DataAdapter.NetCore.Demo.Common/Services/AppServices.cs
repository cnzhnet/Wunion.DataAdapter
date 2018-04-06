using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.SQLite3;
using Wunion.DataAdapter.Kernel.SQLite3.CommandParser;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.Kernel.SQLServer.CommandParser;
using Wunion.DataAdapter.Kernel.MySQL;
using Wunion.DataAdapter.Kernel.MySQL.CommandParser;
using Wunion.DataAdapter.Kernel.PostgreSQL;
using Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser;
using Wunion.DataAdapter.NetCore.Demo.Controllers;

namespace Wunion.DataAdapter.NetCore.Demo
{
    /// <summary>
    /// 用于管理应用程序服务对象的静态类型。
    /// </summary>
    public static class AppServices
    {
        /// <summary>
        /// 获取或设置应用程序的当前工作目录。
        /// </summary>
        public static string ContentRoot { get; set; }

        public static void WriteLog(Exception Ex)
        {
            //System.EventLog
            string LogFile = Path.Combine(ContentRoot, "service_running_err.log");
            if (!(System.IO.File.Exists(LogFile)))
                System.IO.File.Create(LogFile);
            using (TextWriter writer = new StreamWriter(LogFile))
            {
                writer.WriteLine(string.Format("日期：{0}", DateTime.Now));
                writer.WriteLine(string.Format("信息息：{0}", Ex.Message));
                writer.WriteLine(string.Format("工作目录：{0}", ContentRoot));
                writer.WriteLine(Ex.Source);
                writer.WriteLine(Ex.StackTrace);
                writer.WriteLine("---------------------------------------------------------------------------------------------------");
            }
        }

        private static Dictionary<Type, object> DataServices;

        /// <summary>
        /// 获取指定类型的数据支持服务。
        /// </summary>
        /// <typeparam name="T">数据支持服务的目录类型名称</typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            if (DataServices == null)
                DataServices = new Dictionary<Type, object>();
            if (DataServices.ContainsKey(typeof(T)))
            {
                return (T)DataServices[typeof(T)];
            }
            else
            {
                object service = Activator.CreateInstance<T>();
                DataServices.Add(typeof(T), service);
                return (T)service;
            }
        }

        /// <summary>
        /// 初始化要使用的 Wunion.DataAdapter.NetCore 数据引擎。
        /// </summary>
        /// <param name="env">用于获到当前请求 Hotting 信息的对象。</param>
        /// <param name="configuration">用于读取 appsettings.json 配置文件的对象。</param>
        public static void UseDataEngine(IHostingEnvironment env, IConfiguration configuration)
        {
            IConfigurationSection Section = configuration.GetSection("ConnectionStrings");
            SqliteDbAccess SqliteDBA = new SqliteDbAccess();
            string wwwroot = env.WebRootPath;
            if (wwwroot[wwwroot.Length - 1] != '\\')
                wwwroot += "\\";
            SqliteDBA.ConnectionString = Section.GetValue<string>("SQLite").Replace("{wwwroot}", wwwroot);
            DataEngine.AppendDataEngine(SqliteDBA, new SqliteParserAdapter()); // 添加为默认引擎。

            SqlServerDbAccess MsSqlDBA = new SqlServerDbAccess();
            MsSqlDBA.ConnectionString = Section.GetValue<string>("SQLServer");
            DataEngine.AppendDataEngine(MsSqlDBA, new SqlServerParserAdapter(), "SQLServer");

            MySqlDBAccess MySqlDBA = new MySqlDBAccess();
            MySqlDBA.ConnectionString = Section.GetValue<string>("MariaDB");
            DataEngine.AppendDataEngine(MySqlDBA, new MySqlParserAdapter(), "MariaDB");

            NpgsqlDbAccess NpgsqlDBA = new NpgsqlDbAccess();
            NpgsqlDBA.ConnectionString = Section.GetValue<string>("PostgreSQL");
            DataEngine.AppendDataEngine(NpgsqlDBA, new NpgsqlParserAdapter(), "PostgreSQL");
        }
    }
}
