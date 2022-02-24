﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CommandParser;

namespace Wunion.DataAdapter.Kernel.MySQL.CommandParser
{
    /// <summary>
    /// CREATE TABLE 命令解释器.
    /// </summary>
    public class MySqlTableBuildParser : ParserBase
    {
        /// <summary>
        /// 创建一个 <see cref="MySqlTableBuildParser"/> 的对象实例.
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public MySqlTableBuildParser(ParserAdapter adapter) : base(adapter)
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
            StringBuilder fkBuffers = new StringBuilder();
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
                buffers.AppendLine();
                buffers.AppendFormat("\t{0}{1}{2}", ElemIdentifierL, definition.Name, ElemIdentifierR);
                buffers.AppendFormat(" {0} {1}", columnType, definition.NotNull ? "NOT NULL" : "NULL");
                if (definition.Default != null)
                    buffers.AppendFormat(" {0}", ParseDefaultValue(definition, ref DbParameters));
                if (definition.Identity != null)
                {
                    identity = definition.Identity;
                    buffers.Append(" AUTO_INCREMENT");
                }
                if (definition.Unique)
                    buffers.Append(" UNIQUE");
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
                if (definition.ForeignKey != null)
                    ParseForeignKey(tableBuild.Name, definition, fkBuffers);
                if (i < (tableBuild.ColumnDefinitions.Count - 1))
                    buffers.Append(",");
            }
            if (pk_count > 1)
            {
                buffers.Append(",").AppendLine();
                buffers.AppendFormat("\tPRIMARY KEY ({0})", multiPk.ToString());
            }
            if (fkBuffers.Length > 0)
                buffers.Append(",").AppendLine().Append(fkBuffers.ToString());
            buffers.AppendLine();
            if (identity == null)
                buffers.AppendFormat(") ENGINE={0} DEFAULT CHARSET=utf8;", ((MySqlParserAdapter)Adapter).MysqlEngine);
            else
                buffers.AppendFormat(") ENGINE={0} AUTO_INCREMENT={1} DEFAULT CHARSET=utf8;", ((MySqlParserAdapter)Adapter).MysqlEngine, identity.InitValue);
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
                case GenericDbType.NChar:
                    return string.Format("CHAR({0})", definition.Size);
                case GenericDbType.VarChar:
                case GenericDbType.NVarchar:
                    return string.Format("VARCHAR({0})", definition.Size);
                case GenericDbType.Text:
                    return "TEXT";
                case GenericDbType.NText:
                    return "LONGTEXT";
                case GenericDbType.SmallInt:
                    return "SMALLINT";
                case GenericDbType.Int:
                    return "INT";
                case GenericDbType.BigInt:
                    return "BIGINT";
                case GenericDbType.Money:
                    return "DECIMAL";
                case GenericDbType.Single:
                    return "FLOAT";
                case GenericDbType.Double:
                    return "DOUBLE";
                case GenericDbType.Boolean:
                    return "BOOL";
                case GenericDbType.Binary:
                    return "BLOB";
                case GenericDbType.VarBinary:
                    return "MEDIUMBLOB";
                case GenericDbType.Image:
                    return "LONGBLOB";
                case GenericDbType.Time:
                    return "TIME";
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
                        return string.Format("DEFAULT {0}", Convert.ToBoolean(definition.Default) ? 1 : 0);
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
            writer.AppendFormat("\tFOREIGN KEY FK_{0}_{1} ({2}{1}{3})", table, definition.Name, ElemIdentifierL, ElemIdentifierR);
            writer.AppendFormat(" REFERENCES {0}{2}{1} ({0}{3}{1})", ElemIdentifierL, ElemIdentifierR, definition.ForeignKey.Table, definition.ForeignKey.Column);
            writer.AppendFormat(" ON DELETE {0} ON UPDATE {1}", definition.ForeignKey.OnDelete, definition.ForeignKey.OnUpdate);
        }
    }
}
