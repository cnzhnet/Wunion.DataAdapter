using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 表示一个普通的 SQL 命令构建器。
    /// </summary>
    public class DbCommandBuilder : CommandBuilder
    {
        /// <summary>
        /// 实例化一个 <see cref="Wunion.DataAdapter.Kernel.CommandBuilders.DbCommandBuilder"/> 的对象实例。
        /// </summary>
        public DbCommandBuilder()
            : base()
        { }

        /// <summary>
        /// 获取该命令构建器构建的命令类型。
        /// </summary>
        public override System.Data.CommandType CommandType
        {
            get { return System.Data.CommandType.Text; }
        }

        /// <summary>
        /// 用于构建创建表 CREATE TABLE 命令.
        /// </summary>
        /// <param name="name">表名.</param>
        /// <returns></returns>
        public TableBuildDescription CreateTable(string name)
        {
            if (IsParsed)
                ResetCommandBuilder();

            TableBuildDescription tableBuild = new TableBuildDescription(name);
            CommandDescription = tableBuild;
            return tableBuild;
        }

        /// <summary>
        /// 开始构建 INSERT 命令。
        /// </summary>
        /// <param name="table">表信息。</param>
        /// <param name="fields">字段列表。</param>
        /// <returns></returns>
        public InsertBlock Insert(TableDescription table, params FieldDescription[] fields)
        {
            if (IsParsed)
                ResetCommandBuilder();

            InsertBlock Ins = new InsertBlock();
            Ins.Table = table;
            Ins.Fields.AddRange(fields);
            CommandDescription = Ins;
            return Ins;
        }

        /// <summary>
        /// 开始构建 INSERT 命令。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <param name="fields">字段列表。</param>
        /// <returns></returns>
        public InsertBlock Insert(string tableName, params FieldDescription[] fields)
        {
            return Insert(fm.Table(tableName), fields);
        }

        /// <summary>
        /// 开始构建 DELETE 命令。
        /// </summary>
        /// <param name="table">表信息。</param>
        /// <returns></returns>
        public DeleteBlock Delete(TableDescription table)
        {
            if (IsParsed)
                ResetCommandBuilder();

            DeleteBlock del_b = new DeleteBlock();
            del_b.Table = table;
            CommandDescription = del_b;
            return del_b;
        }

        /// <summary>
        /// 开始构建 DELETE 命令。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <returns></returns>
        public DeleteBlock Delete(string tableName)
        {
            return Delete(fm.Table(tableName));
        }

        /// <summary>
        /// 开始构建 UPDATE 命令。
        /// </summary>
        /// <param name="table">表信息。</param>
        /// <returns></returns>
        public UpdateBlock Update(TableDescription table)
        {
            if (IsParsed)
                ResetCommandBuilder();

            UpdateBlock ub = new UpdateBlock();
            ub.AddTable(table);
            CommandDescription = ub;
            return ub;
        }

        /// <summary>
        /// 开始构建 UPDATE 命令。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <returns></returns>
        public UpdateBlock Update(string tableName)
        {
            return Update(fm.Table(tableName));
        }

        /// <summary>
        /// 开始构建 SELECT 命令。
        /// </summary>
        /// <param name="fields">要查询的字段列表（可以为字段信息入表达式对象）。</param>
        /// <returns></returns>
        public SelectBlock Select(params IDescription[] fields)
        {
            if (IsParsed)
                ResetCommandBuilder();

            SelectBlock sb = new SelectBlock();
            foreach (IDescription des in fields)
                sb.AddElement(des);
            CommandDescription = sb;
            return sb;
        }

        /// <summary>
        /// 构建一个用于嵌套查询的子查询 [与Select(...)方法的构建方式一致]。
        /// </summary>
        /// <param name="fields">要查询的字段列表。</param>
        /// <returns></returns>
        public SelectBlock Nested(params IDescription[] fields)
        {
            SelectBlock sblock = new SelectBlock();
            foreach (IDescription des in fields)
                sblock.AddElement(des);
            return sblock;
        }
    }
}
