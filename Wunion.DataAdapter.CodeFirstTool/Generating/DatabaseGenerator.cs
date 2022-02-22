using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.Querying;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using TeleprompterConsole.ProjectAnalysis;

namespace TeleprompterConsole.Generating
{
    /// <summary>
    /// 实现数据库生成器.
    /// </summary>
    public class DatabaseGenerator : Generator
    {
        private DaoCodeGenerator daoGenerator;

        /// <summary>
        /// 创建一个数据库生成器的对象实例.
        /// </summary>
        /// <param name="projRoot">目标项目的根目录.</param>
        /// <param name="lang">语言环境提供程序.</param>
        public DatabaseGenerator(string projRoot, ILanguageProvider lang) : base(projRoot, lang)
        {
            daoGenerator = new DaoCodeGenerator(projRoot, lang);
        }

        /// <summary>
        /// 查询现有数据库架构的版本..
        /// </summary>
        /// <param name="trans">在该事务中执行查询.</param>
        /// <param name="dbc">数据定上下文.</param>
        /// <returns></returns>
        private DbSchemaVersion QueryLastSchema(DBTransactionController trans, DbContext dbc)
        {
            DbSchemaVersion version = null;
            if (trans.DBA.TableExists(DbContext.SCHEMA_VERSION_NAME))
            {
                version = QueryBuilder<DbSchemaVersionDao>.Create(dbc.TableDeclaration<DbSchemaVersion>(DbContext.SCHEMA_VERSION_NAME))
                    .Select(p => p.First.All)
                    .Build((p, options) =>
                    {
                        options.OrderBy(p.First.Version, OrderByMode.DESC);
                        options.Paging(1, 1);
                    })
                    .ToEntityList<DbSchemaVersion>(trans)
                    .FirstOrDefault();
            }
            else
            {
                dbc.CreateTable<DbSchemaVersion>(DbContext.SCHEMA_VERSION_NAME, trans);
            }
            if (version == null)
                version = new DbSchemaVersion { Name = "Wunion DataAdapter CodeFirst", Version = -1 };
            return version;
        }

        /// <summary>
        /// 写入新的数据库架构版本.
        /// </summary>
        /// <param name="trans">在该事务中执行写入操作.</param>
        /// <param name="schema">新的数据库构架版本.</param>
        private void WriteSchemaVersion(DBTransactionController trans, DbContext dbc, DbSchemaVersion schema)
        {
            schema.Creation = DateTime.Now;
            DbTableContext<DbSchemaVersion> tableContext = dbc.TableDeclaration<DbSchemaVersion>(DbContext.SCHEMA_VERSION_NAME);
            tableContext.Add(schema, trans);
        }

        /// <summary>
        /// 运行数据库生成命令.
        /// </summary>
        /// <param name="arg">要生成的数据库上下文定义.</param>
        public override void Run(DbContextDeclaration arg)
        {
            string dbcName = arg.DBC.GetType().Name;
            WriteLog?.Invoke("-------------------------------------");
            WriteLog?.Invoke(Language.GetString("database_generating", dbcName));
            arg.DBC.DbEngine.DBA.ConnectionString = arg.Generating.Database.ConnectionString;
            int skip = 0, reMake = 0, create = 0;
            bool restoreTable = false;
            using (DBTransactionController trans = arg.DBC.DbEngine.BeginTrans())
            {
                GeneratingOptions options = arg.Generating;
                IDbTableMigrator migrator = arg.Generating.TableUpgradeMigrator;
                DbSchemaVersion schema = QueryLastSchema(trans, arg.DBC);
                WriteLog?.Invoke(Language.GetString("upgrade_database_schema", schema.Version, options.Database.SchemaVersion));
                if (schema.Version == options.Database.SchemaVersion)
                {
                    WriteLog?.Invoke(Language.GetString("no_need_upgrade_database"));
                }
                else
                {
                    foreach (DbTableDeclaration table in arg.TableDeclarations)
                    {
                        restoreTable = false;
                        if (trans.DBA.TableExists(table.Name))
                        {
                            if (!arg.Generating.Database.ReCreateExistingTable) //跳过生成已存在的表.
                            {
                                skip++;
                                WriteLog?.Invoke(Language.GetString("skip_generate_table", table.Name));
                                continue;
                            }
                            // 在重建已存在的表前备份表的数据.
                            WriteLog?.Invoke(Language.GetString("backing_up_table", table.Name));
                            migrator?.OnBeforeGenerating(trans, DbTableMigratorEventArgs.Create(table.Name, schema.Version, options.Database.SchemaVersion, WriteLog));
                            restoreTable = true;
                            reMake++;
                            WriteLog?.Invoke(Language.GetString("drop_table", table.Name));
                            trans.DBA.DropTable(table.Name);
                        }
                        WriteLog?.Invoke(Language.GetString("create_table", table.Name));
                        arg.DBC.CreateTable(table.Name, table.EntityType, trans);
                        if (restoreTable)
                            migrator?.OnGenerateCompleted(trans, DbTableMigratorEventArgs.Create(table.Name, schema.Version, options.Database.SchemaVersion, WriteLog));
                        else
                            create++;
                    }
                    schema.Version = options.Database.SchemaVersion;
                    WriteSchemaVersion(trans, arg.DBC, schema);
                    trans.Commit();
                    WriteLog?.Invoke(Language.GetString("database_generate_completed", dbcName, skip, reMake, create));
                }
            }
            arg.DBC.OnGenerateCompleted(WriteLog);
            GenerateQueryDao(arg);
        }

        /// <summary>
        /// 生成实体查询数据访问器.
        /// </summary>
        /// <param name="arg"></param>
        private void GenerateQueryDao(DbContextDeclaration arg)
        {
            daoGenerator.TargetAssembly = TargetAssembly;
            daoGenerator.TargetProject = TargetProject;
            daoGenerator.WriteLog = WriteLog;
            daoGenerator.Run(arg);
        }
    }
}
