using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 用户账户的权限.
    /// </summary>
    public class UserAccountPermission
    {
        /// <summary>
        /// 表示用户账户ID.
        /// </summary>
        [ForeignKey(TableName = "UserAccounts", Field = "UID")]
        [TableField(DbType = GenericDbType.Int, NotNull = true, PrimaryKey = true)]
        public int UID { get; set; }

        /// <summary>
        /// 表示用户账户的权限.
        /// </summary>
        [TableField(DbType = GenericDbType.Text, ValueConverter = typeof(UserPermissionsConverter))]
        public List<string> Permissions { get; set; }
    }

    /// <summary>
    /// 表示用户账户权限集合转换器.
    /// </summary>
    public class UserPermissionsConverter : DbValueConverter<List<string>>
    {
        /// <summary>
        /// 转换到数据库支持的值类型.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dest"></param>
        /// <param name="buffer"></param>
        protected override void ConvertTo(List<string> value, Type dest, out object buffer)
        {
            buffer = JsonSerializer.Serialize<List<string>>(value, new JsonSerializerOptions { 
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), 
                PropertyNamingPolicy = null
            });
        }

        /// <summary>
        /// 从数据库中的值转换为集合.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        protected override void Parse(object value, ref List<string> buffer)
        {
            buffer = JsonSerializer.Deserialize<List<string>>(value.ToString(), new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = null
            });
        }
    }
}
