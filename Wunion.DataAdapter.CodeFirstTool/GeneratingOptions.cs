using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wunion.DataAdapter.Kernel.CodeFirst;

namespace TeleprompterConsole
{
    /// <summary>
    /// 数据库的生成选项设置信息.
    /// </summary>
    public class DbGenerateOption : IDbGenerateOption
    {
        /// <summary>
        /// 创建一个数据库的生成选项设置信息对象实例.
        /// </summary>
        /// <param name="_kind">数据库的种类.</param>
        internal DbGenerateOption(string _kind)
        {
            Kind = _kind;
            ReCreateExistingTable = false;
            SchemaVersion = 0;
        }

        /// <summary>
        /// 目标数据库的连接符串.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 目标数据库的种类.
        /// </summary>
        public string Kind { get; private set; }

        /// <summary>
        /// 删除并重新创建已存在的表.
        /// </summary>
        public bool ReCreateExistingTable { get; set; }

        /// <summary>
        /// 定义当前数据库架构的版本
        /// </summary>
        public int SchemaVersion { get; set; }
    }

    /// <summary>
    /// 用于配置数据库及代码生成选项的对象接口.
    /// </summary>
    public class GeneratingOptions : IGeneratingOptions
    {
        internal GeneratingOptions(IDbGenerateOption dbOption)
        {
            Database = dbOption;
        }

        /// <summary>
        /// 生成时的目标数据库配置选项.
        /// </summary>
        public IDbGenerateOption Database { get; private set; }

        /// <summary>
        /// 生成的实体查询 DAO 数据访问器所使用的命名空间.
        /// </summary>
        public string DaoGenerateNamespace { get; set; }

        /// <summary>
        /// 生成的实体查询 DAO 数据访问器的相对路径（从实体定义所在的目标项目的根目标开始）.
        /// </summary>
        public string DaoGenerateDirectory { get; set; }

        /// <summary>
        /// 用于实现在表升级重建时的数据迁移.
        /// </summary>
        public IDbTableMigrator TableUpgradeMigrator { get; set; }

        internal void ThrowExceptionIfInvalid(ILanguageProvider lang, Type targetDbContext)
        {
            if (string.IsNullOrEmpty(Database.ConnectionString))
                throw new Exception(lang.GetString("db_connection_missing_exception", targetDbContext.Name));
            if (string.IsNullOrEmpty(DaoGenerateNamespace))
                throw new Exception(lang.GetString("dao_namespace_missing_exception", targetDbContext.Name));
            if (string.IsNullOrEmpty(DaoGenerateDirectory))
                throw new Exception(lang.GetString("dao_output_missing_exception", targetDbContext.Name));
        }
    }
}
