using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.EntityUtils.CodeProvider
{
    /// <summary>
    /// 表示类的定义信息.
    /// </summary>
    public class CodeClassDeclaration
    {
        /// <summary>
        /// 创建一个 <see cref="CodeClassDeclaration"/> 的对象实例.
        /// <paramref name="table"/>
        /// </summary>
        public CodeClassDeclaration(List<TableInfoModel> table)
        {
            BaseTypes = new List<string>();
            PropertyMembers = table;
        }

        /// <summary>
        /// 获取或设置类的命名空间.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 获取或设置类名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置表名称.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 获取或设置父类的名称.
        /// </summary>
        public List<string> BaseTypes { get; set; }

        /// <summary>
        /// 获取或设置属性成员信息.
        /// </summary>
        public List<TableInfoModel> PropertyMembers { get; set; }

        /// <summary>
        /// 获取或设置类代码的注释.
        /// </summary>
        public string CodeComment { get; set; }

        /// <summary>
        /// 获取数据库对应的数据类型.
        /// </summary>
        /// <param name="DbTypeName">数据库的数据类型名称.</param>
        /// <returns></returns>
        public string GetTypeSymbol(string DbTypeName)
        {
            switch (DbTypeName.Trim().ToLower())
            {
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "text":
                case "character":
                case "bpchar":
                    return "string";
                case "int2":
                case "smallint":
                case "int16":
                    return "short";
                case "int":
                case "int4":
                case "serial":
                case "integer":
                    return "int";
                case "int8":
                case "bigint":
                case "bigserial":
                    return "long";
                case "real":
                case "float":
                case "float4":
                    return "float";
                case "float8":
                case "double":
                case "money":
                case "numeric":
                    return "double";
                case "decimal":
                    return "Decimal";
                case "bit":
                case "bool":
                case "boolean":
                    return "bool";
                case "time":
                case "timetz":
                    return "object";
                case "date":
                case "datetime":
                case "timestamp":
                    return "DateTime";
                case "bytea":
                case "blob":
                case "longblob":
                case "binary":
                case "varbinary":
                case "image":
                    return "byte[]";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获得数据库类型对应的 Type 对象.
        /// </summary>
        /// <param name="DbTypeName">数据库类型</param>
        /// <returns></returns>
        public Type GetTypeFromDbTpye(string DbTypeName)
        {
            switch (DbTypeName.Trim().ToLower())
            {
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "text":
                case "character":
                case "bpchar":
                    return typeof(string);
                case "int2":
                case "smallint":
                case "int16":
                    return typeof(short);
                case "int":
                case "int4":
                case "serial":
                case "integer":
                    return typeof(int);
                case "int8":
                case "bigint":
                case "bigserial":
                    return typeof(long);
                case "real":
                case "float":
                case "float4":
                    return typeof(float);
                case "float8":
                case "double":
                case "money":
                case "numeric":
                    return typeof(double);
                case "decimal":
                    return typeof(Decimal);
                case "bit":
                case "bool":
                case "boolean":
                    return typeof(bool);
                case "time":
                case "timetz":
                    return typeof(object);
                case "date":
                case "datetime":
                case "timestamp":
                    return typeof(DateTime);
                case "bytea":
                case "blob":
                case "longblob":
                case "binary":
                case "varbinary":
                case "image":
                    return typeof(byte[]);
                default:
                    return null;
            }
        }
    }
}
