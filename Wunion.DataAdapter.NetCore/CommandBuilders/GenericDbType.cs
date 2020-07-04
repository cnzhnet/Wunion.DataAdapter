using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.CommandBuilders
{
    /// <summary>
    /// 表示通用数据库类型.
    /// </summary>
    public enum GenericDbType
    {
        /// <summary>
        /// 16 位整数类型.
        /// </summary>
        SmallInt = 0,
        /// <summary>
        /// 32 位整数类型.
        /// </summary>
        Int = 1,
        /// <summary>
        /// 64 位整数类型.
        /// </summary>
        BigInt = 2,
        /// <summary>
        /// 8 字节货币类型
        /// </summary>
        Money = 3,
        /// <summary>
        /// 32 位单精度浮点数.
        /// </summary>
        Single = 4,
        /// <summary>
        /// 64 位双精度浮点数.
        /// </summary>
        Double = 5,
        /// <summary>
        /// 定长字符.
        /// </summary>
        Char = 6,
        /// <summary>
        /// 变长字符.
        /// </summary>
        VarChar = 7,
        /// <summary>
        /// 标准 Unicode 定长度符.
        /// </summary>
        NChar = 8,
        /// <summary>
        /// 标准 Unicode 变长字符.
        /// </summary>
        NVarchar = 9,
        /// <summary>
        /// 超长文本类型.
        /// </summary>
        Text = 10,
        /// <summary>
        /// 标准 Unicode 超长文本类型（使用 MySQL 时对应 longtext 类型）.
        /// </summary>
        NText = 11,
        /// <summary>
        /// 布尔类型.
        /// </summary>
        Boolean = 12,
        /// <summary>
        /// 定长二进制数据（使用 MySQL 时对应 BLOB 类型）.
        /// </summary>
        Binary = 13,
        /// <summary>
        /// 变长二进制数据（使用 MySQL 时对应 MEDIUMBLOB 类型）.
        /// </summary>
        VarBinary = 14,
        /// <summary>
        /// 大容量的二进制数据（使用 MySQL 时对应 LONGBLOB 类型）.
        /// </summary>
        Image = 15,
        /// <summary>
        /// 日间类型通常用于一天内（不带时区，若在使用 SQLite 数据库时指定该类型将导致异常）.
        /// </summary>
        Time = 16,
        /// <summary>
        /// 日期类型（不含日间）.
        /// </summary>
        Date = 17,
        /// <summary>
        /// 日期时间类型.
        /// </summary>
        DateTime = 18
    }
}
