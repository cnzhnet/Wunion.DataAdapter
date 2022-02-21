using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.DbInterop;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于实现在 CodeFirst 重建表时的数据迁移.
    /// </summary>
    public interface IDbTableMigrator
    {
        /// <summary>
        /// 在重建表前执行的操作（通常用于查询并备份表中的数据）.
        /// </summary>
        /// <param name="trans">用于执行命令的事务控制器.</param>
        /// <param name="e">本次表迁移相关的参数信息.</param>
        void OnBeforeGenerating(DBTransactionController trans, DbTableMigratorEventArgs e);

        /// <summary>
        /// 在重建表完成后执行的操作（通常用于重新写入备份的数据）.
        /// </summary>
        /// <param name="trans">用于执行命令的事务控制器.</param>
        /// <param name="e">本次表迁移相关的参数信息.</param>
        void OnGenerateCompleted(DBTransactionController trans, DbTableMigratorEventArgs e);
    }

    /// <summary>
    /// 用于数据表迁移事件的参数传递.
    /// </summary>
    public class DbTableMigratorEventArgs : EventArgs
    {
        private Action<string> logger;

        /// <summary>
        /// 创建一个 <see cref="DbTableMigratorEventArgs"/> 的对象实例.
        /// </summary>
        /// <param name="table">表名.</param>
        /// <param name="ver">现有的架架构版本.</param>
        /// <param name="newVer">将要升级到的架构版本.</param>
        /// <param name="_logger">用于输出日志.</param>
        private DbTableMigratorEventArgs(string table, int ver, int newVer, Action<string> _logger)
        {
            tableName = table;
            SchemaVersion = ver;
            UpgradeVersion = newVer;
            logger = _logger;
        }

        /// <summary>
        /// 正在迁移的表名称.
        /// </summary>
        public string tableName { get; private set; }

        /// <summary>
        /// 现有的数据库架构版本.
        /// </summary>
        public int SchemaVersion { get; private set; }

        /// <summary>
        /// 要升级到的数据库版本.
        /// </summary>
        public int UpgradeVersion { get; private set; }

        /// <summary>
        /// 向 CodeFirst 工具输出迁移日志.
        /// </summary>
        /// <param name="message">要输出的日志消息.</param>
        public void Log(string message) => logger?.Invoke(message);

        /// <summary>
        /// 创建一个迁移事件参数.
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="existingVer">现有的架构版本</param>
        /// <param name="newVer"><将要升级到的架构版本/param>
        /// <param name="logger">用于输出日志</param>
        /// <returns></returns>
        public static DbTableMigratorEventArgs Create(string tblName, int existingVer, int newVer, Action<string> logger)
        {
            return new DbTableMigratorEventArgs(tblName, existingVer, newVer, logger);
        }
    }
}
