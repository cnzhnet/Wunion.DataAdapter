using System;
using System.Collections.Generic;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.Querying;


namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 表示用户账户实体. DAO .
    /// </summary>
    public class UserAccountDao : QueryDao
    {
        public UserAccountDao(DbContext dbc = null) : base(dbc)
        { }

        public UserAccountDao() : base(null)
        { }

        public override Type EntityType => typeof(UserAccount);

        /// <summary>
        /// 表示用户账户ID.
        /// </summary>
        public FieldDescription UID => GetField("UID");

        /// <summary>
        /// 用户账户名称.
        /// </summary>
        public FieldDescription Name => GetField("Name");

        /// <summary>
        /// 用户密码.
        /// </summary>
        public FieldDescription Password => GetField("Password");

        /// <summary>
        /// 用户账户的状态.
        /// </summary>
        public FieldDescription Status => GetField("Status");

        /// <summary>
        /// 该用户账户的使用人.
        /// </summary>
        public FieldDescription User => GetField("User");

        /// <summary>
        /// 手机号.
        /// </summary>
        public FieldDescription PhoneNumber => GetField("PhoneNumber");

        /// <summary>
        /// 电子邮件.
        /// </summary>
        public FieldDescription Email => GetField("Email");

        /// <summary>
        /// 该用户账户的创建日期.
        /// </summary>
        public FieldDescription Creation => GetField("Creation");

        protected override IDbTableContext GetTableContext(string name)
        {
            if (string.IsNullOrEmpty(name))
                return db?.TableDeclaration<UserAccount>("UserAccounts");
            return db?.TableDeclaration<UserAccount>(name);
        }

        protected override IDescription[] GetAllFields()
        {
            return new IDescription[] {
                UID,
                Name,
                Password,
                Status,
                User,
                PhoneNumber,
                Email,
                Creation
            };
        }
    }
}
