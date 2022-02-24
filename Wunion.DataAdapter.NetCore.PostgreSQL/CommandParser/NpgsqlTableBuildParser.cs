using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.PostgreSQL.CommandParser
{
    /// <summary>
    /// CREATE TABLE 命令解释器.
    /// </summary>
    public class NpgsqlTableBuildParser : ParserBase
    {
        /// <summary>
        /// 创建一个 <see cref="NpgsqlTableBuildParser"/> 的对象实例.
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public NpgsqlTableBuildParser(ParserAdapter adapter) : base(adapter)
        { }

        /// <summary>
        /// 解释 CREATE TABLE 命令.
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            TableBuildDescription tableBuild = (TableBuildDescription)this.Description;
            StringBuilder tableBuffers = new StringBuilder("CREATE TABLE ");
            StringBuilder sequnceBuffers = new StringBuilder();
            StringBuilder pkBuffers = new StringBuilder();
            StringBuilder uniqueBuffers = new StringBuilder();
            StringBuilder fkBuffers = new StringBuilder();
            tableBuffers.AppendFormat("{0}{1}{2} (", ElemIdentifierL, tableBuild.Name, ElemIdentifierR);
            string columnType = null, seqName = null;
            DbTableColumnDefinition definition = null;
            for (int i = 0; i < tableBuild.ColumnDefinitions.Count; ++i)
            {
                definition = tableBuild.ColumnDefinitions[i];
                if (string.IsNullOrEmpty(definition.Name))
                    throw new NoNullAllowedException("Undefined column name.");
                columnType = ParseDbType(definition);
                if (string.IsNullOrEmpty(columnType))
                    throw new NoNullAllowedException(string.Format("Type of undefined column: {0}", definition.Name));
                tableBuffers.AppendLine().AppendFormat("\t{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                tableBuffers.AppendFormat(" {0} {1}", columnType, definition.NotNull ? "NOT NULL" : "NULL");
                if (definition.Default != null)// && definition.Identity == null
                    tableBuffers.AppendFormat(" {0}", ParseDefaultValue(definition, ref DbParameters));
                if (definition.Identity != null)
                {
                    seqName = string.Format("{0}_{1}_seq", tableBuild.Name, definition.Name);
                    AddSequnce(definition.Identity, seqName, sequnceBuffers);
                    tableBuffers.AppendFormat(" DEFAULT nextval('{0}{1}{2}'::regclass)", ElemIdentifierL, seqName, ElemIdentifierR);
                }
                if (definition.Unique)
                {
                    if (uniqueBuffers.Length > 0)
                        uniqueBuffers.AppendFormat(",{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                    else
                        uniqueBuffers.AppendFormat("{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                }
                if (definition.PrimaryKey)
                {
                    if (pkBuffers.Length > 0)
                        pkBuffers.AppendFormat(",{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                    else
                        pkBuffers.AppendFormat("{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                }
                if (definition.ForeignKey != null)
                    ParseForeignKey(tableBuild.Name, definition, fkBuffers);
                if (i < (tableBuild.ColumnDefinitions.Count - 1))
                    tableBuffers.Append(",");
            }
            if (pkBuffers.Length > 0)
            {
                tableBuffers.Append(",").AppendLine();
                tableBuffers.AppendFormat("\tCONSTRAINT {0}PK_{1}{2}", ElemIdentifierL, tableBuild.Name, ElemIdentifierR);
                tableBuffers.AppendFormat(" PRIMARY KEY ({0})", pkBuffers.ToString());
            }
            if (uniqueBuffers.Length > 0)
            {
                tableBuffers.Append(",").AppendLine();
                tableBuffers.AppendFormat("\tCONSTRAINT {0}UK_{1}_UNIQUE{2}", ElemIdentifierL, tableBuild.Name, ElemIdentifierR);
                tableBuffers.AppendFormat(" UNIQUE ({0})", uniqueBuffers.ToString());
            }
            if (fkBuffers.Length > 0)
                tableBuffers.Append(",").AppendLine().Append(fkBuffers.ToString());
            tableBuffers.AppendLine().Append(");");
            return sequnceBuffers.AppendLine().Append(tableBuffers.ToString()).ToString();
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
                case GenericDbType.NChar:
                    return string.Format("character({0})", definition.Size);
                case GenericDbType.VarChar:
                case GenericDbType.NVarchar:
                    return string.Format("character varying({0})", definition.Size);
                case GenericDbType.Text:
                case GenericDbType.NText:
                    return "text";
                case GenericDbType.SmallInt:
                    return "smallint";
                case GenericDbType.Int:
                    return "integer";
                case GenericDbType.BigInt:
                    return "bigint";
                case GenericDbType.Money:
                    return "decimal";
                case GenericDbType.Single:
                    return "real";
                case GenericDbType.Double:
                    return "double precision";
                case GenericDbType.Boolean:
                    return "boolean";
                case GenericDbType.Binary:
                case GenericDbType.VarBinary:
                case GenericDbType.Image:
                    return "bytea";
                case GenericDbType.Time:
                    return (definition.Size > 0) ? string.Format("time({0})", definition.Size) : "TIME";
                case GenericDbType.Date:
                    return "date";
                case GenericDbType.DateTime:
                    return "timestamp";
            }
            return string.Empty;
        }

        /// <summary>
        /// 添加一个自增长序列.
        /// </summary>
        /// <param name="identity">自增长的设置信息.</param>
        /// <param name="seq_name">自增长序列的名称.</param>
        /// <param name="writer">自增长序列生成到该缓冲区.</param>
        private void AddSequnce(DbColumnIdentity identity, string seq_name, StringBuilder writer)
        {
            if (writer.Length > 0)
                writer.AppendLine();
            writer.AppendFormat("CREATE SEQUENCE {0}{1}{2}", ElemIdentifierL, seq_name, ElemIdentifierR).AppendLine();
            writer.AppendFormat("\tINCREMENT {0} ", identity.Increment).AppendLine();
            writer.AppendFormat("\tSTART {0}", identity.InitValue).AppendLine();
            writer.AppendFormat("\tMINVALUE {0}", identity.InitValue).AppendLine();
            writer.Append("\tCACHE 1;");
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
                        return string.Format("DEFAULT {0}", Convert.ToBoolean(definition.Default) ? "TRUE" : "FALSE");
                    case GenericDbType.SmallInt:
                    case GenericDbType.Int:
                    case GenericDbType.BigInt:
                    case GenericDbType.Single:
                    case GenericDbType.Money:
                    case GenericDbType.Double:
                        return string.Format("DEFAULT {0}", definition.Default);
                    case GenericDbType.Binary:
                    case GenericDbType.VarBinary:
                    case GenericDbType.Image:
                        throw new NotSupportedException(string.Format("Data type {0} does not support setting default value.", definition.Default));
                    default:
                        return string.Format("DEFAULT '{0}'", definition.Default.ToString());
                }
            }
            valueDes.DescriptionParserAdapter = this.Adapter;
            return string.Format("DEFAULT {0}", valueDes.GetParser().Parsing(ref DbParameters));
        }

        /// <summary>
        /// 解析列的外键设置.
        /// </summary>
        /// <param name="table">当前表名.</param>
        /// <param name="definition">列定义信息.</param>
        /// <param name="writer">将外键命令段写入该缓冲区.</param>
        /// <returns></returns>
        private void ParseForeignKey(string table, DbTableColumnDefinition definition, StringBuilder writer)
        {
            if (writer.Length > 0)
                writer.Append(",").AppendLine();
            writer.AppendFormat("\tCONSTRAINT {2}FK_{0}_{1}{3} FOREIGN KEY ({2}{1}{3})", table, definition.Name, ElemIdentifierL, ElemIdentifierR);
            writer.AppendFormat(" REFERENCES {0}{2}{1} ({0}{3}{1})", ElemIdentifierL, ElemIdentifierR, definition.ForeignKey.Table, definition.ForeignKey.Column);
            writer.AppendFormat(" ON DELETE {0} ON UPDATE {1}", definition.ForeignKey.OnDelete, definition.ForeignKey.OnUpdate);
        }
    }
}
