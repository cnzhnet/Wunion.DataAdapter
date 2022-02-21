using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 数据库上下文对象.
    /// </summary>
    public abstract class DbContext
    {
        private ConcurrentDictionary<string, IDbTableContext> _tables;
        /// <summary>
        /// 架构信息表的名称.
        /// </summary>
        public const string SCHEMA_VERSION_NAME = "WDA_SCHEMA_VERSIONS";

        /// <summary>
        /// 创建一个数据库上下文对象.
        /// </summary>
        /// <param name="engine">数据库引擎对象.</param>
        protected DbContext(DataEngine engine)
        {
            DbEngine = engine;
            _tables = new ConcurrentDictionary<string, IDbTableContext>();
        }

        internal ConcurrentDictionary<string, IDbTableContext> Tables => _tables;

        /// <summary>
        /// 获取数据库引擎对象.
        /// </summary>
        public DataEngine DbEngine { get; private set; }

        /// <summary>
        /// 获取或定义数据库中的表.
        /// </summary>
        /// <typeparam name="TEntity">表记录对应的实体类型.</typeparam>
        /// <param name="tableName">获取已定义的表时上参数应为 null，定义数据库表时则必须指定此参数.</param>
        /// <returns></returns>
        public DbTableContext<TEntity> TableDeclaration<TEntity>(string tableName = null) where TEntity : class, new()
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            Type t = typeof(TEntity);
            IDbTableContext context;
            if (Tables.TryGetValue(tableName, out context))
                return (DbTableContext<TEntity>)context;
            DbTableContext<TEntity> tmp = new DbTableContext<TEntity>(this, tableName);
            Tables.TryAdd(tableName, tmp);
            return tmp;
        }

        /// <summary>
        /// 获取为指定类型的实体在数据库中创建表的命令.
        /// </summary>
        /// <param name="entityType">实体的类型.</param>
        /// <param name="tableName">表名称.</param>
        /// <returns></returns>
        private DbCommandBuilder GetCreateTableCommand(Type entityType, string tableName)
        {
            // 先获取实体类型的所有表字段映射.
            List<DbConversionMapping> mappings = DbConversionMapping.Get(entityType);
            if (mappings.Count < 1)
                throw new Exception($"The entity class “{entityType.FullName}” does not define any public properties.");
            //根据实体类的属性标记定义表的字段.
            List<DbTableColumnDefinition> columnDefinitions = new List<DbTableColumnDefinition>();
            foreach (DbConversionMapping mp in mappings)
            {
                if (string.IsNullOrEmpty(mp.Attribute.Name))
                    mp.Attribute.Name = mp.Property.Name;
                columnDefinitions.Add(
                    DbTableColumnDefinition.New(
                        mp.Attribute.Name,
                        mp.Attribute.DbType,
                        mp.Attribute.Size,
                        mp.Attribute.NotNull,
                        mp.Attribute.Unique,
                        mp.Attribute.PrimaryKey,
                        mp.Attribute.Default,
                        mp.Identity,
                        mp.ForeignKey
                ));
            }
            // 创建表.
            DbCommandBuilder cb = new DbCommandBuilder();
            cb.CreateTable(tableName).ColumnsDefine(columnDefinitions.ToArray());
            return cb;
        }

        /// <summary>
        /// 若在与数据库交互的过程中产生了错误时，检测并触发异常.
        /// </summary>
        /// <param name="sender">产生错误的 <see cref="DataEngine"/> 或 <see cref="DBTransactionController"/> 对象.</param>
        /// <param name="result">数据库返回的执行结果状态码（值为非 -1 则表示未产生错误）.</param>
        public void ThrowIfDbException(object sender, int result)
        {
            if (result != -1)
                return;
            DataEngine engine = (sender == null) ? DbEngine : sender as DataEngine;
            if (engine != null)
                throw new Exception(engine.DBA.Error.Message);
            // 判断是否为事务中产生的异常.
            DBTransactionController trans = sender as DBTransactionController;
            if (trans != null)
                throw new Exception(trans.DBA.Errors.Last().Message);
        }

        /// <summary>
        /// 在数据库中创建表.
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="enityType">定义该表的实体类型.</param>
        /// <param name="dest">表示执行此操作的 <see cref="BatchCommander"/> 批处理器或 <see cref="DBTransactionController"/>事务控制器，为空则表示不在批处理或事务中执行.</param>
        public void CreateTable(string tableName, Type enityType, object dest = null)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));
            DbCommandBuilder cb = GetCreateTableCommand(enityType, tableName);
            if (dest == null) // 不在批处理及事务中执行.
            {
                ThrowIfDbException(null, DbEngine.ExecuteNoneQuery(cb));
                return;
            }

            DBTransactionController trans = dest as DBTransactionController;
            if (trans != null) // 在事务中执行.
            {
                ThrowIfDbException(trans, trans.DBA.ExecuteNoneQuery(cb));
                return;
            }

            BatchCommander batch = dest as BatchCommander;
            if (batch != null) // 在批处理中执行.
            {
                batch.ExecuteNonQuery(cb);
                return;
            }
            throw new ArgumentException("Invalid argument: \"dest\", only objects of type DBTransactionController or BatchCommander are supported");
        }

        /// <summary>
        /// 在该数据库中创建表.
        /// </summary>
        /// <typeparam name="TEntity">该表对应的实体类型.</typeparam>
        /// <param name="tableName">表名.</param>
        /// <param name="dest">表示执行此操作的 <see cref="BatchCommander"/> 批处理器或 <see cref="DBTransactionController"/>事务控制器，为空则表示不在批处理或事务中执行.</param>
        public void CreateTable<TEntity>(string tableName, object dest = null) where TEntity : class
        {
            CreateTable(tableName, typeof(TEntity), dest);
        }

        /// <summary>
        /// 从数据库中删除指定的表.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dest"></param>
        public void DropTable(string tableName, object dest = null)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (dest == null) // 不在事务或批处理中执行.
            {
                DbEngine.DBA.DropTable(tableName);
                return;
            }

            DBTransactionController trans = dest as DBTransactionController;
            if (trans != null) // 在事务中执行.
            {
                ThrowIfDbException(trans, trans.DBA.DropTable(tableName) ? 1 : -1);
                return;
            }

            BatchCommander batch = dest as BatchCommander;
            if (batch != null)
            {
                batch.DropTable(tableName);
                return;
            }
            throw new ArgumentException("Invalid argument: \"dest\", only objects of type DBTransactionController or BatchCommander are supported");
        }

        /// <summary>
        /// 用于实现在创建或更新数据库架构前要执行的操作（若未正确实现此方法则 CodeFirst 工具将无法连接到数据库）.
        /// </summary>
        /// <param name="options">生成数据库架构及实体查询数据访问器代码所必须的选项设置.</param>
        public abstract void OnBeforeGenerating(IGeneratingOptions options);

        /// <summary>
        /// 当 CodeFirst 工具完成数据库生成时调用此方法（用于向数据库中预置数据）.
        /// </summary>
        /// <param name="log">用于输出日志.</param>
        public virtual void OnGenerateCompleted(Action<string> log)
        { }
    }
}
