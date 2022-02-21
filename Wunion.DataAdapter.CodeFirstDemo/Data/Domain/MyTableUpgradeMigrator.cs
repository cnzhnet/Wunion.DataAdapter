using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.DbInterop;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 用于在表升级重建时迁移数据.
    /// </summary>
    public class MyTableUpgradeMigrator : IDbTableMigrator
    {
        public MyTableUpgradeMigrator()
        { }

        public void OnBeforeGenerating(DBTransactionController trans, DbTableMigratorEventArgs e)
        {
            if (e.SchemaVersion == e.UpgradeVersion)
                return;
            e.Log($"[{e.tableName}]：哈哈，没有需要迁移的数据.");
        }

        public void OnGenerateCompleted(DBTransactionController trans, DbTableMigratorEventArgs e)
        {
            // TODO
        }
    }
}
