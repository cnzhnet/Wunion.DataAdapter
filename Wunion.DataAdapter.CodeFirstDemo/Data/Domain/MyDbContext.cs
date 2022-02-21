using System;
using System.Collections.Generic;
using System.Linq;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.DbInterop;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 测试的数据库.
    /// </summary>
    public class MyDbContext : DbContext
    {
        /// <summary>
        /// 创建一个 <see cref="MyDbContext"/> 数据库上下文对象实例.
        /// </summary>
        /// <param name="engine"></param>
        public MyDbContext(DataEngine engine) : base(engine)
        { }

        /// <summary>
        /// 用户账户表.
        /// </summary>
        public DbTableContext<UserAccount> UserAccounts => TableDeclaration<UserAccount>("UserAccounts");

        /// <summary>
        /// 用户账户权限表.
        /// </summary>
        [GenerateOrder(1)]
        public DbTableContext<UserAccountPermission> UserPermissions => TableDeclaration<UserAccountPermission>("UserPermissions");

        //// <summary>
        /// 用于实现在创建或更新数据库架构前要执行的操作（若未正确实现此方法则 CodeFirst 工具将无法连接到数据库）.
        /// </summary>
        /// <param name="options">生成数据库架构及实体查询数据访问器代码所必须的选项设置.</param>
        public override void OnBeforeGenerating(IGeneratingOptions options)
        {
            switch (options.Database.Kind)
            {
                case "sqlite3":
                    options.Database.ConnectionString = @"Data Source=D:\SQLiteStudio\wda-codefirst.db";
                    break;
            }
            options.Database.SchemaVersion = 1; //当前的数据库架构版本.
            options.Database.ReCreateExistingTable = true;
            options.TableUpgradeMigrator = new MyTableUpgradeMigrator();
            options.DaoGenerateNamespace = "Wunion.DataAdapter.CodeFirstDemo.Data.Domain";
            options.DaoGenerateDirectory = System.IO.Path.Combine("Data", "Domain", "DAO");
        }
    }
}
