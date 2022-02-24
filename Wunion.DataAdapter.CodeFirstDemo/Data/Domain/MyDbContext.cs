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
        public DbTableContext<UserAccountGroup> UserAccountGroups => TableDeclaration<UserAccountGroup>("UserAccountGroups");

        //// <summary>
        /// 用于实现在创建或更新数据库架构前要执行的操作（若未正确实现此方法则 CodeFirst 工具将无法连接到数据库）.
        /// </summary>
        /// <param name="options">生成数据库架构及实体查询数据访问器代码所必须的选项设置.</param>
        public override void OnBeforeGenerating(IGeneratingOptions options)
        {
            switch (options.Database.Kind)
            {
                case "mssql":
                    options.Database.ConnectionString = "Server=192.168.1.2;Database=Wunion.DataAdapter.CodeFirstDemo;User ID=sa;Password=kcbg7-hb8x2;";
                    break;
                case "sqlite3":
                    options.Database.ConnectionString = @"Data Source=D:\SQLiteStudio\wda-codefirst.db";
                    break;
                case "mysql":
                    options.Database.ConnectionString = "Data Source=app01.ksdemo.cn;Database=long_range;User ID=long_range; Password=P8iT2hekJPZ27Xzt;";
                    break;
                case "npgsql":
                    options.Database.ConnectionString = "Host=192.168.1.11;Username=postgres;Password=lengyifan;Database=Wunion.DataAdapter.CodeFirstDemo;";
                    break;
            }
            options.Database.SchemaVersion = 1; //当前的数据库架构版本.
            options.Database.ReCreateExistingTable = true;
            options.TableUpgradeMigrator = new MyTableUpgradeMigrator();
            options.DaoGenerateNamespace = "Wunion.DataAdapter.CodeFirstDemo.Data.Domain";
            options.DaoGenerateDirectory = System.IO.Path.Combine("Data", "Domain", "DAO");
        }

        /// <summary>
        /// 在数据库架构生成完成时创建预置数据.
        /// </summary>
        /// <param name="log"></param>
        public override void OnGenerateCompleted(Action<string> log)
        {
            log("正在生成预置数据 ...");
            using (BatchCommander batch = new BatchCommander(DbEngine))
            {
                try
                {
                    DateTime creation = new DateTime(2022, 2, 22, 22, 57, 49);
                    // 预置用户账户组.
                    log("正在预置管理员用户组 ...");
                    UserAccountGroup group = new UserAccountGroup { Id = 100, Name = "Admin", Description = "管理员用户组", Creation = creation };
                    UserAccountGroups.Add(group, batch);
                    group.Id = Convert.ToInt32(batch.SCOPE_IDENTITY);
                    // 预置用户账户.
                    log("正在预置超级用户 ...");
                    UserAccount ua = new UserAccount {
                        Name = "super-admin",
                        Password = "mE9nJTgxBo3lDGJOg47LzX42a89K+LvjbeAGQyfpG5k=",
                        Status = UserAccountStatus.Enabled,
                        Groups = new List<int>(new int[] { group.Id }),
                        User = "巨陽道君",
                        Email = "cnzhnet@hotmail.com",
                        Creation = creation
                    };
                    UserAccounts.Add(ua, batch);
                    log("数据预置已顺利完成.");
                }
                catch (Exception Ex)
                {
                    log(Ex.Message);
                    log(Ex.StackTrace);
                }
            }
        }
    }
}
