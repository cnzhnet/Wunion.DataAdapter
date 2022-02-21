using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel.MySQL
{
    /// <summary>
    /// 用于声明 MySQL 数据库的存储引擎规范.
    /// </summary>
    public static class StorageEngine
    {
        /// <summary>
        /// 归档存储引擎允许将大量用于归档目的的记录存储为压缩格式以节省磁盘空间。 归档存储引擎在插入时压缩记录，并在读取时使用zlib库对其进行解压缩。
        /// </summary>
        public const string ARCHIVE = "ARCHIVE";

        /// <summary>
        /// CSV存储引擎以逗号分隔值(CSV)文件格式存储数据。 CSV表格提供了将数据迁移到非SQL应用程序(如电子表格软件)中的便捷方式。
        /// </summary>
        public const string CSV = "CSV";

        /// <summary>
        /// FEDERATED存储引擎允许从远程MySQL服务器管理数据，而无需使用集群或复制技术。本地联合表不存储任何数据。 从本地联合表查询数据时，数据将从远程联合表自动拉出。
        /// </summary>
        public const string FEDERATED = "FEDERATED";

        /// <summary>
        /// InnoDB表完全支持符合ACID和事务。 它们也是性能最佳的。InnoDB表支持外键，提交，回滚，前滚操作。InnoDB表的大小最多可达64TB。
        /// </summary>
        public const string INNODB = "INNODB";

        /// <summary>
        /// MyISAM扩展了以前的ISAM存储引擎，MyISAM表的大小可达256TB，这个数据里是非常巨大的。 此外，MyISAM表可以压缩为只读表以节省空间。 
        /// 在启动时，MySQL会检查MyISAM表是否有损坏，甚至在出现错误的情况下修复它们。MyISAM表不是事务安全的。
        /// </summary>
        public const string MY_ISAM = "MyISAM";

        /// <summary>
        /// MERGE表是将具有相似结构的多个MyISAM表组合到一个表中的虚拟表。MERGE存储引擎也被称为MRG_MyISAM引擎。 MERGE表没有自己的索引，它会使用组件表的索引。
        /// </summary>
        public const string MERGE = "MERGE";

        /// <summary>
        /// 内存表存储在内存中，并使用散列索引，使其比MyISAM表格快。内存表数据的生命周期取决于数据库服务器的正常运行时间。内存存储引擎以前称为HEAP。
        /// </summary>
        public const string MEMORY = "MEMORY";
    }
}
