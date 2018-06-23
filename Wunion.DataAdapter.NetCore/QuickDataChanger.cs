using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.DataCollection;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 扩展快捷数据更改器的批量提交标识。
    /// </summary>
    public enum QuickDataChangerSubmission
    {
        /// <summary>
        /// 表示忽略该数据。
        /// </summary>
        Ignore = 0x0,
        /// <summary>
        /// 表示插入数据。
        /// </summary>
        Insert = 0x1,
        /// <summary>
        /// 表示更新数据。
        /// </summary>
        Update = 0x2,
        /// <summary>
        /// 表示删除数据。
        /// </summary>
        Delete = 0x4
    }

    /// <summary>
    /// 表示数据行回写库时的提交方式。
    /// </summary>
    public class DataRowSubmission
    {
        private QuickDataChangerSubmission _Submission;
        private List<object> _Conditions;
        private string[] _NotInsertFields;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DataRowSubmission"/> 的对象实例。
        /// </summary>
        /// <param name="submission">数据行的提交方式。</param>
        public DataRowSubmission(QuickDataChangerSubmission submission)
        {
            _Submission = submission;
            _Conditions = new List<object>();
        }

        /// <summary>
        /// 获取数据行的提交方式。
        /// </summary>
        public QuickDataChangerSubmission Submission
        {
            get { return _Submission; }
        }

        /// <summary>
        /// 获取或设置作为更新或删除数据行的条件（当是插入操作时不必设置。但如果更新和删除时不设置该属性，则会被忽略）。
        /// </summary>
        public List<object> Conditions
        {
            get { return _Conditions; }
        }

        /// <summary>
        /// 获取或设置更新或插入操作时不对其进行写操作的字段（更新时不改的字段，或插入时不赋值的字段）。
        /// </summary>
        public string[] NotInsertFields
        {
            get { return _NotInsertFields; }
            set { _NotInsertFields = value; }
        }
    }

    /// <summary>
    /// 用于在执行批量更新
    /// </summary>
    /// <param name="Row"></param>
    /// <returns></returns>
    public delegate DataRowSubmission BatchSaveDataRowJudgment(DataRow Row);

    /// <summary>
    /// 用于在执行批量更新
    /// </summary>
    /// <param name="Row"></param>
    /// <returns></returns>
    public delegate DataRowSubmission BatchSaveSpeedRowJudgment(SpeedDataRow Row);

    /// <summary>
    /// 扩展快捷数据更改器。使用该类可避免手动通过 DbCommandBuilder 来建筑 INSERT 或 UPDATE 命令。
    /// </summary>
    public class QuickDataChanger
    {
        private List<object> _Conditions;
        private DBTransactionController Trans;
        private DataEngine _Engine;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.QuickDataChanger"/> 的对象实例。
        /// </summary>
        /// <param name="engine">执行数据库交互的引擎对象。</param>
        public QuickDataChanger(DataEngine engine = null)
        {
            _Conditions = new List<object>();
            Trans = null;
            if (engine == null)
                _Engine = DataEngine.CurrentEngine;
            else
                _Engine = engine;
        }

        /// <summary>
        /// 获取数据引擎。
        /// </summary>
        protected DataEngine Engine
        {
            get { return _Engine; }
        }

        /// <summary>
        /// 创建一个与事务关联的 <see cref="Wunion.DataAdapter.Kernel.QuickDataChanger"/> 对象实例。
        /// </summary>
        /// <param name="transController">关联到该对象的事务控制器。</param>
        /// <param name="engine">执行数据库交互的引擎对象。</param>
        public QuickDataChanger(DBTransactionController transController, DataEngine engine = null)
        {
            _Conditions = new List<object>();
            Trans = transController;
            if (engine == null)
                _Engine = DataEngine.CurrentEngine;
            else
                _Engine = engine;
        }

        /// <summary>
        /// 添加用于更新数据时的条件，元素的语法与： DbCommander.Update().Where() 方法的参数中的元素语法一致。
        /// 例：
        /// <para>Conditions.Add(td.Field("field1") == "测试");</para>
        /// <para>Conditions.Add(exp.And);</para>
        /// <para>Conditions.Add(Fun.BetweenAnd(td.Field("createdate"), time1, time2));</para>
        /// </summary>
        public List<object> Conditions
        {
            get { return _Conditions; }
        }

        /// <summary>
        /// 插入数据。
        /// </summary>
        /// <param name="TableName">表名称。</param>
        /// <param name="data">包含要插入的数据的字典（字典的键必与数据库目标表的字段一致）。</param>
        /// <returns></returns>
        protected int Insert(string TableName, Dictionary<string, object> data)
        {
            List<FieldDescription> Fields = new List<FieldDescription>();
            List<object> Values = new List<object>();
            foreach (KeyValuePair<string, object> item in data)
            {
                Fields.Add(td.Field(item.Key));
                if (item.Value == null)
                    Values.Add(DBNull.Value);
                else
                    Values.Add(item.Value);
            }
            DbCommandBuilder Command = new DbCommandBuilder();
            Command.Insert(fm.Table(TableName), Fields.ToArray()).Values(Values.ToArray());
            if (Trans == null)
                return Engine.DBA.ExecuteNoneQuery(Command);
            else
                return Trans.DBA.ExecuteNoneQuery(Command);
        }

        /// <summary>
        /// 更新数据。
        /// </summary>
        /// <param name="TableName">表名称。</param>
        /// <param name="data">包含要插入的数据的字典（字典的键必与数据库目标表的字段一致）。</param>
        /// <returns></returns>
        protected int Update(string TableName, Dictionary<string, object> data)
        {
            List<IDescription> expressions = new List<IDescription>();
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value == null)
                    expressions.Add(td.Field(item.Key) == DBNull.Value);
                else
                    expressions.Add(td.Field(item.Key) == item.Value);
            }
            DbCommandBuilder Command = new DbCommandBuilder();
            if (Conditions != null)
                Command.Update(fm.Table(TableName)).Set(expressions.ToArray()).Where(Conditions.ToArray());
            else
                Command.Update(fm.Table(TableName)).Set(expressions.ToArray());
            if (Trans == null)
                return Engine.DBA.ExecuteNoneQuery(Command);
            else
                return Trans.DBA.ExecuteNoneQuery(Command);
        }

        /// <summary>
        /// 将数据保存到数据库中。
        /// </summary>
        /// <param name="TableName">表名称。</param>
        /// <param name="data">包含要插入的数据的字典（字典的键必与数据库目标表的字段一致）。</param>
        /// <param name="executeUpdate">更新模式。为 true 时将以更新的方式执行，如果此时没有设置 Conditions 条件，则更个更新将没有条件约束。为 false 时为插入数据。</param>
        /// <returns></returns>
        public int SaveToDataBase(string TableName, Dictionary<string, object> data, bool executeUpdate)
        {
            if (executeUpdate)
                return Update(TableName, data);
            else
                return Insert(TableName, data);
        }

        /// <summary>
        /// 从删除。
        /// </summary>
        /// <param name="TableName">要从中删除数据的表（条件请指定 Conditions 属性）。</param>
        /// <returns></returns>
        public int Delete(string TableName)
        {
            DbCommandBuilder Command = new DbCommandBuilder();
            if (Conditions != null)
                Command.Delete(fm.Table(TableName)).Where(Conditions.ToArray());
            else
                Command.Delete(fm.Table(TableName));
            if (Trans == null)
                return Engine.DBA.ExecuteNoneQuery(Command);
            else
                return Trans.DBA.ExecuteNoneQuery(Command);
        }

        /// <summary>
        /// 检查指定的字段是否在不插入（或更新）的字段列表中。
        /// </summary>
        /// <param name="rowSubmission">对该行的处理指示器。</param>
        /// <param name="Field">要检查的字段名称。</param>
        /// <returns>若指定的字段包含在不插入（或更新）的字段列表中时返回 true, 否则返回 false</returns>
        private bool NotInsertFieldsContains(DataRowSubmission rowSubmission, string Field)
        {
            if (rowSubmission.NotInsertFields == null)
                return false;
            int index = Array.IndexOf<string>(rowSubmission.NotInsertFields, Field);
            return index != -1;
        }

        /// <summary>
        /// 判断并构建指定行被指示的提交命令。
        /// </summary>
        /// <param name="Row">被提交的行。</param>
        /// <param name="rowSubmission">对该行的处理指示。</param>
        /// <returns></returns>
        private DbCommandBuilder RowSubmissionCommand(SpeedDataRow Row, DataRowSubmission rowSubmission)
        {
            if (rowSubmission == null)
                return null;
            // 开始构建命令。
            DbCommandBuilder Command = new DbCommandBuilder();
            switch (rowSubmission.Submission)
            {
                case QuickDataChangerSubmission.Delete:
                    if (rowSubmission.Conditions != null && rowSubmission.Conditions.Count > 0)
                        Command.Delete(Row.Table.TableName).Where(rowSubmission.Conditions.ToArray());
                    break;
                case QuickDataChangerSubmission.Update:
                    if (rowSubmission.Conditions != null && rowSubmission.Conditions.Count > 0)
                    {
                        List<IDescription> expressions = new List<IDescription>();
                        foreach (SpeedDataColumn c in Row.Table.Columns)
                        {
                            if (!NotInsertFieldsContains(rowSubmission, c.Name))
                            {
                                if (Row[c.Name] == null)
                                    expressions.Add(td.Field(c.Name) == DBNull.Value);
                                else
                                    expressions.Add(td.Field(c.Name) == Row[c.Name]);
                            }
                        }
                        Command.Update(Row.Table.TableName).Set(expressions.ToArray()).Where(rowSubmission.Conditions.ToArray());
                    }
                    break;
                case QuickDataChangerSubmission.Insert:
                    List<FieldDescription> Fields = new List<FieldDescription>();
                    List<object> Values = new List<object>();
                    foreach (SpeedDataColumn c in Row.Table.Columns)
                    {
                        if (!NotInsertFieldsContains(rowSubmission, c.Name))
                        {
                            Fields.Add(td.Field(c.Name));
                            if (Row[c.Name] == DBNull.Value)
                                Values.Add(DBNull.Value);
                            else
                                Values.Add(Row[c.Name]);
                        }
                    }
                    Command.Insert(fm.Table(Row.Table.TableName), Fields.ToArray()).Values(Values.ToArray());
                    break;
                default:
                    break;
            }
            return Command;
        }

        /// <summary>
        /// 将一个 DataTable 中的改动批量保存回数据库（仅针对单表查询所得的数据集，并且在调用批量更新之前必须将数据库中的要更新的表名称设置为 DataTable 对像的 TableName 属性）。
        /// </summary>
        /// <param name="Table">需要对其进行批量保存的数据集。</param>
        /// <param name="rowJudgment">用于判断数据行的操作处理的函数，在该函数中您需要判断给定的 Table 的某个字段在当前行的值。并通过该值来告诉程序应该如何处理此行。
        /// <para>例如：在 DataTable 中设置一个名为“STATUS”的状态列表示更新状态。</para>
        /// <para>      0 表示未无改动，1 表示新增， 2 表示更新（任何一列的数据已被更改），3 表示删除（用户进行了删除时标记为此值，而不是直接从Table中删除）</para>
        /// <para>示例代码请参见更新日志中。</para>
        /// </param>
        /// <returns></returns>
        public int BatchSaveDataTable(SpeedDataTable Table, BatchSaveSpeedRowJudgment rowJudgment)
        {
            if (Table == null)
                return 0;
            if (Table.Count < 1)
                return 0;
            if (string.IsNullOrEmpty(Table.TableName))
                throw new Exception("在未指定 Table.TableName 的情况下不能使用批量更新。您必须将 Table 对象的 TableName 属性指定为数据库中的表名称时批量更新方能正常工作。");
            if (rowJudgment == null)
                throw new Exception("不指定 rowJudgment 方法时批量更新方法无法工作。");
            int effect = 0;
            DataRowSubmission rowSubmission;
            for (int i = 0; i < Table.Count; ++i)
            {
                rowSubmission = rowJudgment(Table[i]);
                if (rowSubmission == null)
                    rowSubmission = new DataRowSubmission(QuickDataChangerSubmission.Ignore);
                if (rowSubmission.Submission == QuickDataChangerSubmission.Ignore)
                    continue;
                DbCommandBuilder Command = RowSubmissionCommand(Table[i], rowSubmission);
                if (Command == null)
                    continue;
                if (Trans == null)
                    effect += Engine.DBA.ExecuteNoneQuery(Command);
                else
                    effect += Trans.DBA.ExecuteNoneQuery(Command);
            }
            return effect;
        }

        #region 对System.Data.DataTable的支持

        /// <summary>
        /// 判断并构建指定行被指示的提交命令。
        /// </summary>
        /// <param name="Row">被提交的行。</param>
        /// <param name="rowSubmission">对该行的处理指示。</param>
        /// <returns></returns>
        private DbCommandBuilder DataRowSubmissionCommand(DataRow Row, DataRowSubmission rowSubmission)
        {
            if (rowSubmission == null)
                return null;
            // 开始构建命令。
            DbCommandBuilder Command = new DbCommandBuilder();
            switch (rowSubmission.Submission)
            {
                case QuickDataChangerSubmission.Delete:
                    if (rowSubmission.Conditions != null && rowSubmission.Conditions.Count > 0)
                        Command.Delete(Row.Table.TableName).Where(rowSubmission.Conditions.ToArray());
                    break;
                case QuickDataChangerSubmission.Update:
                    if (rowSubmission.Conditions != null && rowSubmission.Conditions.Count > 0)
                    {
                        List<IDescription> expressions = new List<IDescription>();
                        foreach (DataColumn c in Row.Table.Columns)
                        {
                            if (!NotInsertFieldsContains(rowSubmission, c.ColumnName))
                            {
                                if (Row[c.ColumnName] == null)
                                    expressions.Add(td.Field(c.ColumnName) == DBNull.Value);
                                else
                                    expressions.Add(td.Field(c.ColumnName) == Row[c.ColumnName]);
                            }
                        }
                        Command.Update(Row.Table.TableName).Set(expressions.ToArray()).Where(rowSubmission.Conditions.ToArray());
                    }
                    break;
                case QuickDataChangerSubmission.Insert:
                    List<FieldDescription> Fields = new List<FieldDescription>();
                    List<object> Values = new List<object>();
                    foreach (DataColumn c in Row.Table.Columns)
                    {
                        if (!NotInsertFieldsContains(rowSubmission, c.ColumnName))
                        {
                            Fields.Add(td.Field(c.ColumnName));
                            if (Row[c.ColumnName] == DBNull.Value)
                                Values.Add(DBNull.Value);
                            else
                                Values.Add(Row[c.ColumnName]);
                        }
                    }
                    Command.Insert(fm.Table(Row.Table.TableName), Fields.ToArray()).Values(Values.ToArray());
                    break;
                default:
                    break;
            }
            return Command;
        }

        /// <summary>
        /// 将一个 DataTable 中的改动批量保存回数据库（仅针对单表查询所得的数据集，并且在调用批量更新之前必须将数据库中的要更新的表名称设置为 DataTable 对像的 TableName 属性）。
        /// </summary>
        /// <param name="Table">需要对其进行批量保存的数据集。</param>
        /// <param name="rowJudgment">用于判断数据行的操作处理的函数，在该函数中您需要判断给定的 DataTable 的某个字段在当前行的值。并通过该值来告诉程序应该如何处理此行。
        /// <para>例如：在 DataTable 中设置一个名为“STATUS”的状态列表示更新状态。</para>
        /// <para>      0 表示未无改动，1 表示新增， 2 表示更新（任何一列的数据已被更改），3 表示删除（用户进行了删除时标记为此值，而不是直接从DataTable中删除）</para>
        /// <para>示例代码请参见更新日志中。</para>
        /// </param>
        /// <returns></returns>
        public int BatchSaveDataTable(DataTable Table, BatchSaveDataRowJudgment rowJudgment)
        {
            if (Table == null)
                return 0;
            if (Table.Rows.Count < 1)
                return 0;
            if (string.IsNullOrEmpty(Table.TableName))
                throw new Exception("在未指定 Table.TableName 的情况下不能使用批量更新。您必须将 Table 对象的 TableName 属性指定为数据库中的表名称时批量更新方能正常工作。");
            if (rowJudgment == null)
                throw new Exception("不指定 rowJudgment 方法时批量更新方法无法工作。");
            int effect = 0;
            DataRowSubmission rowSubmission;
            for (int i = 0; i < Table.Rows.Count; ++i)
            {
                rowSubmission = rowJudgment(Table.Rows[i]);
                if (rowSubmission == null)
                    rowSubmission = new DataRowSubmission(QuickDataChangerSubmission.Ignore);
                if (rowSubmission.Submission == QuickDataChangerSubmission.Ignore)
                    continue;
                DbCommandBuilder Command = DataRowSubmissionCommand(Table.Rows[i], rowSubmission);
                if (Command == null)
                    continue;
                if (Trans == null)
                    effect += Engine.DBA.ExecuteNoneQuery(Command);
                else
                    effect += Trans.DBA.ExecuteNoneQuery(Command);
            }
            return effect;
        }
        #endregion
    }
}
