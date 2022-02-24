using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.SQLite3.CommandParser
{
    /// <summary>
    /// CREATE TABLE 命令的解释器.
    /// </summary>
    public class SqliteTableBuildParser : ParserBase
    {
        /// <summary>
        /// 创建一个 <see cref="SqliteTableBuildParser"/> 的对象实例.
        /// </summary>
        /// <param name="adapter"></param>
        public SqliteTableBuildParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释 CREATE TABLE 命令.
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            TableBuildDescription tableBuild = (TableBuildDescription)this.Description;
            StringBuilder buffers = new StringBuilder("CREATE TABLE ");
            StringBuilder multiPk = new StringBuilder();
            buffers.AppendFormat("{0}{1}{2} (", ElemIdentifierL, tableBuild.Name, ElemIdentifierR);
            string columnType = null;
            int pk_count = tableBuild.ColumnDefinitions.Count(def => def.PrimaryKey == true);
            DbTableColumnDefinition definition = null;
            DbColumnIdentity identity = null;
            for (int i = 0; i < tableBuild.ColumnDefinitions.Count; ++i)
            {
                definition = tableBuild.ColumnDefinitions[i];
                if (string.IsNullOrEmpty(definition.Name))
                    throw new NoNullAllowedException("Undefined column name.");
                columnType = ParseDbType(definition);
                if (string.IsNullOrEmpty(columnType))
                    throw new NoNullAllowedException(string.Format("Type of undefined column: {0}", definition.Name));
                buffers.AppendLine().AppendFormat("\t{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                buffers.AppendFormat(" {0} {1}", columnType, definition.NotNull ? "NOT NULL" : "NULL");
                if (definition.Unique)
                    buffers.Append(" UNIQUE");
                if (definition.Default != null)
                    buffers.AppendFormat(" {0}", ParseDefaultValue(definition, ref DbParameters));
                if (definition.PrimaryKey)
                {
                    if (pk_count > 1) // 联合主键判定.
                    {
                        if (multiPk.Length > 0)
                            multiPk.AppendFormat(",{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                        else
                            multiPk.AppendFormat("{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                    }
                    else
                    {
                        buffers.Append(" PRIMARY KEY");
                    }
                }                
                if (definition.Identity != null)
                {
                    if (!definition.PrimaryKey)
                        throw new NotSupportedException(string.Format("AUTOINCREMENT can't be applied to non-primary key column: {0}", definition.Name));
                    if (pk_count > 1)
                        throw new NotSupportedException(string.Format("AUTOINCREMENT can't be applied to multi-primary key columns.", definition.Name));
                    buffers.Append(" AUTOINCREMENT");
                    identity = definition.Identity;
                }
                if (definition.ForeignKey != null)
                    ParseForeignKey(definition, buffers);
                if (i < (tableBuild.ColumnDefinitions.Count - 1))
                    buffers.Append(",");
            }
            if (pk_count > 1)
            {
                buffers.Append(",").AppendLine().AppendFormat("\tPRIMARY KEY ({0})", multiPk.ToString());
            }
            buffers.AppendLine().AppendLine(");");
            if (identity != null)
                buffers.AppendLine(string.Format("UPDATE sqlite_sequence SET seq = {0} WHERE name = '{1}';", identity.InitValue - 1, tableBuild.Name));
            return buffers.ToString();
        }

        /// <summary>
        /// 解析指定列定义的数据类型.
        /// </summary>
        /// <param name="definition">列定义信息.</param>
        /// <returns></returns>
        private string ParseDbType(DbTableColumnDefinition definition)
        {
            switch (definition.DataType)
            {
                case GenericDbType.Char:
                    return string.Format("CHARACTER({0})", definition.Size);
                case GenericDbType.NChar:
                    return string.Format("NCHAR({0})", definition.Size);
                case GenericDbType.VarChar:
                    return string.Format("VARCHAR({0})", definition.Size);
                case GenericDbType.NVarchar:
                    return string.Format("NVARCHAR({0})", definition.Size);
                case GenericDbType.Text:
                case GenericDbType.NText:
                    return "TEXT";
                case GenericDbType.SmallInt:
                    return "SMALLINT";
                case GenericDbType.Int:
                    return "INTEGER";
                case GenericDbType.BigInt:
                    return "BIGINT";
                case GenericDbType.Money:
                    return "DECIMAL(10,5)";
                case GenericDbType.Single:
                    return "FLOAT";
                case GenericDbType.Double:
                    return "DOUBLE";
                case GenericDbType.Boolean:
                    return "BOOLEAN";
                case GenericDbType.Binary:
                case GenericDbType.VarBinary:
                case GenericDbType.Image:
                    return "BLOB";
                case GenericDbType.Time:
                    throw new NotSupportedException(string.Format("SQLite unsupported this type: {0}", definition.DataType));
                case GenericDbType.Date:
                    return "DATE";
                case GenericDbType.DateTime:
                    return "DATETIME";
            }
            return string.Empty;
        }

        /// <summary>
        /// 列的解析默认值设置.
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="DbParameters"></param>
        /// <returns></returns>
        private string ParseDefaultValue(DbTableColumnDefinition definition, ref List<IDbDataParameter> DbParameters)
        {
            IDescription valueDes = definition.Default as IDescription;
            if (valueDes == null)
            {
                switch (definition.DataType)
                {
                    case GenericDbType.Boolean:
                        return string.Format("DEFAULT({0})", Convert.ToBoolean(definition.Default) ? 1 : 0);
                    case GenericDbType.SmallInt:
                    case GenericDbType.Int:
                    case GenericDbType.BigInt:
                    case GenericDbType.Single:
                    case GenericDbType.Money:
                    case GenericDbType.Double:
                        return string.Format("DEFAULT({0})", definition.Default);
                    case GenericDbType.Binary:
                    case GenericDbType.VarBinary:
                    case GenericDbType.Image:
                        throw new NotSupportedException(string.Format("Data type {0} does not support setting default value.", definition.Default));
                    default:
                        return string.Format("DEFAULT('{0}')", definition.Default.ToString());
                }
            }
            valueDes.DescriptionParserAdapter = this.Adapter;
            return string.Format("DEFAULT({0})", valueDes.GetParser().Parsing(ref DbParameters));
        }

        /// <summary>
        /// 解析列的外键设置.
        /// </summary>
        /// <param name="definition">列定义信息.</param>
        /// <param name="writer">将外键命令段写入该缓冲区.</param>
        /// <returns></returns>
        private void ParseForeignKey(DbTableColumnDefinition definition, StringBuilder writer)
        {
            writer.AppendFormat(" REFERENCES {0}{1}{2}", ElemIdentifierL, definition.ForeignKey.Table, ElemIdentifierR);
            writer.AppendFormat(" ({0}{1}{2})", ElemIdentifierL, definition.ForeignKey.Column, ElemIdentifierR);
            writer.AppendFormat(" ON DELETE {0} ON UPDATE {1}", definition.ForeignKey.OnDelete, definition.ForeignKey.OnUpdate);
        }
    }
}
