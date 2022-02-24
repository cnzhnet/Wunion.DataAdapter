using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 表示用户账户表的实体
    /// </summary>
    public class UserAccountGroup : WriteDateSoftDelete
    {
        /// <summary>
        /// 组 ID
        /// </summary>
        [Identity(0, 1)]
        [TableField(DbType = GenericDbType.Int, NotNull = true, PrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 组名称.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 64, NotNull = true, Unique = true)]
        public string Name { get; set; }

        /// <summary>
        /// 组说明.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 255)]
        public string Description { get; set; }

        /// <summary>
        /// 该组的权限集.
        /// </summary>
        [TableField(DbType = GenericDbType.Text, ValueConverter = typeof(IntegerCollectionConverter))]
        public List<int> Permissions { get; set; }
    }

    /// <summary>
    /// 表示用户账户权限集合转换器.
    /// </summary>
    public class IntegerCollectionConverter : DbValueConverter<List<int>>
    {
        /// <summary>
        /// 转换到数据库支持的值类型.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dest"></param>
        /// <param name="buffer"></param>
        protected override void ConvertTo(List<int> value, Type dest, out object buffer)
        {
            buffer = JsonSerializer.Serialize<List<int>>(value, new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = null
            });
        }

        /// <summary>
        /// 从数据库中的值转换为集合.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        protected override void Parse(object value, ref List<int> buffer)
        {
            if (value == null || DBNull.Value.Equals(value))
                return;

            buffer = JsonSerializer.Deserialize<List<int>>(value.ToString(), new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = null
            });
        }
    }
}
