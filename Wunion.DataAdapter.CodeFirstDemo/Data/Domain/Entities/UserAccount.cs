using System;
using System.Collections.Generic;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 表示用户账户的状态.
    /// </summary>
    public enum UserAccountStatus
    { 
        /// <summary>
        /// 正常启用状态.
        /// </summary>
        Enabled = 0,
        /// <summary>
        /// 用户账户已被禁用.
        /// </summary>
        Disabled = 1,
        /// <summary>
        /// 用户账户已被锁定.
        /// </summary>
        Locked = 2
    }

    /// <summary>
    /// 表示用户账户实体.
    /// </summary>
    public class UserAccount : WriteDateTime
    {
        /// <summary>
        /// 表示用户账户ID.
        /// </summary>
        [Identity(0, 1)]
        [TableField(DbType = GenericDbType.Int, PrimaryKey = true, NotNull = true)]
        public int UID { get; set; }

        /// <summary>
        /// 用户账户名称.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 32, NotNull = true, Unique = true)]
        public string Name { get; set; }

        /// <summary>
        /// 用户密码.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 64, NotNull = true)]
        public string Password { get; set; }

        /// <summary>
        /// 用户账户的状态.
        /// </summary>
        [TableField(DbType = GenericDbType.Int, Default = (int)UserAccountStatus.Enabled, NotNull = true, ValueConverter = typeof(UserAccountStatusConverter))]
        public UserAccountStatus Status { get; set; }

        /// <summary>
        /// 该用户隶属的组.
        /// </summary>
        [TableField(DbType = GenericDbType.Text, NotNull = true, ValueConverter = typeof(IntegerCollectionConverter))]
        public List<int> Groups { get; set; }

        /// <summary>
        /// 该用户账户的使用人.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 32)]
        public string User { get; set; }

        /// <summary>
        /// 手机号.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 12)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 电子邮件.
        /// </summary>
        [TableField(DbType = GenericDbType.VarChar, Size = 255)]
        public string Email { get; set; }
    }

    /// <summary>
    /// 用户账户状态枚举的数据库类型转换器.
    /// </summary>
    public class UserAccountStatusConverter : DbValueConverter<UserAccountStatus>
    {
        /// <summary>
        /// 转换到数据库支持的类型.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dest"></param>
        /// <param name="buffer"></param>
        protected override void ConvertTo(UserAccountStatus value, Type dest, out object buffer)
        {
            if (dest == typeof(string)) // 转换为字符串.
            {
                buffer = Enum.GetName<UserAccountStatus>(value);
                return;
            }    
            // 转换为数字.
            buffer = (int)value;
        }

        /// <summary>
        /// 从数据库支持的类型转换为枚举.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        protected override void Parse(object value, ref UserAccountStatus buffer)
        {
            // 从数字转换.
            if (value.GetType() == typeof(int))
            {
                int numeric = Convert.ToInt32(value);
                buffer = (UserAccountStatus)numeric;
                return;
            }
            // 从字符串转换.
            string name = value.ToString();
            buffer = Enum.Parse<UserAccountStatus>(name);
        }
    }
}
