using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    public interface IDbGenerateOption
    { 
        /// <summary>
        /// 目标数据库的种类（mssql | mysql | npgsql | sqlite3）.
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// 目标数据库的连接符串.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 删除并重新创建已存在的表.
        /// </summary>
        bool ReCreateExistingTable { get; set; }

        /// <summary>
        /// 用于定义数据库架构的版本号.
        /// </summary>
        int SchemaVersion { get; set; }
    }

    /// <summary>
    /// 用于配置数据库及代码生成选项的对象接口.
    /// </summary>
    public interface IGeneratingOptions
    {
        /// <summary>
        /// 生成时的目标数据库配置选项.
        /// </summary>
        IDbGenerateOption Database { get; }

        /// <summary>
        /// 生成的实体查询 DAO 数据访问器所使用的命名空间.
        /// </summary>
        string DaoGenerateNamespace { get; set; }

        /// <summary>
        /// 生成的实体查询 DAO 数据访问器的相对路径（从实体定义所在的目标项目的根目标开始）.
        /// </summary>
        string DaoGenerateDirectory { get; set; }

        /// <summary>
        /// 用于实现在表升级重建时的数据迁移.
        /// </summary>
        IDbTableMigrator TableUpgradeMigrator { get; set; }
    }
}
