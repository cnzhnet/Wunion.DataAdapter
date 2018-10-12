using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.DataCollection;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 用于执行数据交互命令的引擎对象。
    /// </summary>
    public class DataEngine
    {
        private DbAccess _DBA;
        private ParserAdapter _CommandParserAdapter;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataEngine"/> 的对象实例。
        /// </summary>
        /// <param name="dba">该引擎使用的数据访问器。</param>
        /// <param name="parserAdapter">解释命令时使用的适配器。</param>
        public DataEngine(DbAccess dba, ParserAdapter parserAdapter)
        {
            _DBA = dba;
            _CommandParserAdapter = parserAdapter;

            if (DBA != null)
                DBA.parserAdapter = parserAdapter;
        }

        /// <summary>
        /// 获取或设置该引擎实例所使用的数据交互访问器对象。
        /// </summary>
        public DbAccess DBA
        {
            get { return _DBA; }
        }

        /// <summary>
        /// 获取该引擎解释命令所需要的解释适配器。
        /// </summary>
        public ParserAdapter CommandParserAdapter
        {
            get { return _CommandParserAdapter; }
        }

        /// <summary>
        /// 执行指定的 SQL 命令，并返回受影响的记录数。
        /// </summary>
        /// <param name="Command">CommandBuilder对象。</param>
        /// <returns></returns>
        public int ExecuteNoneQuery(CommandBuilder Command)
        {
            return DBA.ExecuteNoneQuery(Command);
        }

        /// <summary>
        /// 执行指定的查询命令，并返回数据集。
        /// </summary>
        /// <param name="Command">CommandBuilder对象。</param>
        /// <returns></returns>
        public T ExecuteQuery<T>(CommandBuilder Command) where T : DataTable
        {
            DataTable dt = DBA.ExecuteQuery<DataTable>(Command);
            return (T)dt;
        }

        /// <summary>
        /// 执行指定的查询命令，并返回 <see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataTable"/> 数据集。
        /// </summary>
        /// <typeparam name="T"><see cref="Wunion.DataAdapter.Kernel.DataCollection.SpeedDataTable"/>类型参数。</typeparam>
        /// <param name="Command">CommandBuilder对象。</param>
        /// <returns></returns>
        public SpeedDataTable ExecuteQuery(CommandBuilder Command)
        {
            return DBA.ExecuteQuery(Command);
        }

        /// <summary>
        /// 执行指定的查询，并返回该查询对应的动态实体对象数据集合.
        /// </summary>
        /// <param name="Command">要执行的查询命令对象.</param>
        /// <exception cref="Exception">在查询过程中产生错误时引发此异常.</exception>
        /// <returns>查询对应的动态实体对象数据集合.</returns>
        public List<dynamic> ExecuteDynamicEntity(CommandBuilder Command)
        {
            return DBA.ExecuteDynamicEntity(Command);
        }

        /// <summary>
        /// 执行指定的查询命令，并返回相应的数据读取器。
        /// </summary>
        /// <param name="Command">CommandBuilder对象。</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandBuilder Command)
        {
            return DBA.ExecuteReader(Command);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="Command">CommandBuilder对象。</param>
        /// <returns></returns>
        public object ExecuteScalar(CommandBuilder Command)
        {
            return DBA.ExecuteScalar(Command);
        }

        /// <summary>
        /// 开启事务处理。
        /// </summary>
        /// <returns></returns>
        public DBTransactionController BeginTrans()
        {
            if (DBA == null || CommandParserAdapter == null)
                throw (new Exception("无法开启事务，因为尚未初始化 DBA 或 CommandParserAdapter 对象。"));
            IDbConnection DbConnection = DBA.Connect();
            DbConnection.Open();
            IDbTransaction Trans = DbConnection.BeginTransaction();
            IDbCommand DbCommand = DBA.CreateDbCommand();
            DbCommand.Connection = DbConnection;
            DbCommand.Transaction = Trans;
            TransactionDbAccess transDBA = new TransactionDbAccess(DbCommand, this);
            DBTransactionController TransController = new DBTransactionController(Trans, transDBA, new ReleaseTransHandler(ReleaseTransaction));
            TransactionConnectionCache.RegisterValidTransaction(TransController.UniqueId, DbConnection);
            return TransController;
        }

        /// <summary>
        /// 释放与事务相关的资源（包含数据连接）。
        /// </summary>
        /// <param name="Owner">事务的所有者。</param>
        public void ReleaseTransaction(DBTransactionController Owner)
        {
            TransactionConnectionCache.ReleaseConnection(Owner.UniqueId);
        }

        #region 静态成员

        private static Dictionary<string, DataEngine> DataEnginePools; // 数据交互引擎池。
        private static int _CurrentEngineStatus;

        /// <summary>
        /// 获取或设置当前的数据交互引擎对象。
        /// </summary>
        public static DataEngine CurrentEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 获取一个值，该值指示当前引擎的状态。（该属性用于检查当前引擎是否是默认的引擎，如果不是则可通过调用 EndChange() 方法来回到默认引擎上）。
        /// </summary>
        public static int CurrentEngineStatus
        {
            get { return _CurrentEngineStatus; }
        }

        /// <summary>
        /// 向引擎池中添加一个引擎(注：需要在组件的支持范围内)。
        /// </summary>        
        /// <param name="Dba">该引擎与数据库进行交互的访问器。</param>
        /// <param name="parserAdapter">该引擎用的解析命令的适配器。</param>
        /// <param name="key">该引擎在引擎池中的键名称。</param>
        public static void AppendDataEngine(DbAccess Dba, ParserAdapter parserAdapter, string key = "Default")
        {
            if (DataEnginePools == null)
                DataEnginePools = new Dictionary<string, DataEngine>();
            if (DataEnginePools.ContainsKey(key))
                return;
            DataEngine Engine = new DataEngine(Dba, parserAdapter);
            DataEnginePools.Add(key, Engine);
            if (key.ToUpper() == "DEFAULT")
                DataEngine.CurrentEngine = Engine;
        }

        /// <summary>
        /// 清理引擎池中，除默认引擎外的所有其他引擎。
        /// </summary>
        public static void EnginePoolsClean()
        {
            if (DataEnginePools == null)
                return;
            DataEngine Default = DataEnginePools["Default"];
            DataEnginePools.Clear();
            DataEnginePools.Add("Default", Default);
            // 清理完成后将当前引擎设置为默认引擎，不然如果当前引擎已被切换为默认引擎以外的其它引擎时就会出错。
            DataEngine.CurrentEngine = Default;
            _CurrentEngineStatus = ENGINE_IS_DEFAULT;
        }

        /// <summary>
        /// 从引擎池中移除指定的绵里藏针。
        /// </summary>
        /// <param name="database">引擎在池内对应的数据库类型名称“键”。</param>
        public static void RemoveEngine(string database)
        {
            if (DataEnginePools == null)
                return;
            if (database == "Default") // 禁止移除默认引擎（不然无法工作了）。
                return;
            if (DataEnginePools.ContainsKey(database))
                DataEnginePools.Remove(database);
            // 同样，如果此处不处理。当前引擎如果就是该引擎的话，被移除后就会出错。
            DataEngine.CurrentEngine = DataEnginePools["Default"];
            _CurrentEngineStatus = ENGINE_IS_DEFAULT;
        }

        /// <summary>
        /// 使当前引擎切换到指定的数据库引擎，成功则返回 DataEngine.ENGINE_CHANGE_OK 常量。当在一个程序中需要访问多种数据库时，使用引擎池来实现。随时可以使用该方法
        /// 将当前引擎切换到其它数据库引擎上，并在使用完成后调用 EndChange() 方法撤销切换。
        /// </summary>
        /// <param name="database">要切换到的数据库交互引擎在引擎池中的键名称。</param>
        /// <returns></returns>
        public static int ChangeEngine(string database)
        {
            if (DataEnginePools == null)
                return ENGINE_POOLS_IS_EMPTY;
            if (DataEnginePools.ContainsKey(database))
            {
                DataEngine.CurrentEngine = DataEnginePools[database];
                _CurrentEngineStatus = ENGINE_IS_NOT_DEFAULT;
                return ENGINE_CHANGE_OK;
            }
            else
            {
                return ENGINE_NOT_INSTANCE;
            }
        }

        /// <summary>
        /// 从数据库引擎池中获取指定键的引擎对象。
        /// </summary>
        /// <param name="key">引擎实例在池中的键名称。</param>
        /// <returns></returns>
        public static DataEngine GetEngine(string key)
        {
            if (DataEnginePools.ContainsKey(key))
                return DataEnginePools[key];
            else
                return null;
        }

        /// <summary>
        /// 撤销 ChangeEngine 方法对当前引擎所做的切换操作。
        /// </summary>
        public static void EndChange()
        {
            if (DataEnginePools == null)
                return;
            DataEngine.CurrentEngine = DataEnginePools["Default"];
            _CurrentEngineStatus = ENGINE_IS_DEFAULT;
        }

        #endregion

        #region 常量定义

        /// <summary>
        /// 引擎尚未创建。
        /// </summary>
        public const int ENGINE_NOT_INSTANCE = 0x800F;
        /// <summary>
        /// 引擎池是空的。
        /// </summary>
        public const int ENGINE_POOLS_IS_EMPTY = 0x800E;
        /// <summary>
        /// 表示当前引擎已不是默认引擎。
        /// </summary>
        public const int ENGINE_IS_NOT_DEFAULT = 0x800D;
        /// <summary>
        /// 表示当前引擎是默认引擎。
        /// </summary>
        public const int ENGINE_IS_DEFAULT = 0x800C;
        /// <summary>
        /// 当前引擎已顺利切换。
        /// </summary>
        public const int ENGINE_CHANGE_OK = 0x800B;

        #endregion
    }
}
