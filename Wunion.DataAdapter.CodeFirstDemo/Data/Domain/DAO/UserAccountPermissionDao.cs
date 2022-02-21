using System;
using System.Collections.Generic;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.Querying;


namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 用户账户的权限. DAO .
    /// </summary>
    public class UserAccountPermissionDao : QueryDao
    {
        public UserAccountPermissionDao(DbContext dbc = null) : base(dbc)
        { }

        public UserAccountPermissionDao() : base(null)
        { }

        public override Type EntityType => typeof(UserAccountPermission);

        /// <summary>
        /// 表示用户账户ID.
        /// </summary>
        public FieldDescription UID => GetField("UID");

        /// <summary>
        /// 表示用户账户的权限.
        /// </summary>
        public FieldDescription Permissions => GetField("Permissions");

        protected override IDbTableContext GetTableContext(string name)
        {
            if (string.IsNullOrEmpty(name))
                return db?.TableDeclaration<UserAccountPermission>("UserPermissions");
            return db?.TableDeclaration<UserAccountPermission>(name);
        }

        protected override IDescription[] GetAllFields()
        {
            return new IDescription[] {
                UID,
                Permissions
            };
        }
    }
}
