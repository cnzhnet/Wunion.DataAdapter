using System;
using System.Collections.Generic;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.Kernel.CodeFirst;
using Wunion.DataAdapter.Kernel.Querying;


namespace Wunion.DataAdapter.CodeFirstDemo.Data.Domain
{
    /// <summary>
    /// 表示用户账户表的实体 DAO .
    /// </summary>
    public class UserAccountGroupDao : QueryDao
    {
        public UserAccountGroupDao(DbContext dbc = null) : base(dbc)
        { }

        public UserAccountGroupDao() : base(null)
        { }

        public override Type EntityType => typeof(UserAccountGroup);

        /// <summary>
        /// 组ID
        /// </summary>
        public FieldDescription Id => GetField("Id");

        /// <summary>
        /// 组名称.
        /// </summary>
        public FieldDescription Name => GetField("Name");

        /// <summary>
        /// 组说明.
        /// </summary>
        public FieldDescription Description => GetField("Description");

        /// <summary>
        /// 该组的权限集.
        /// </summary>
        public FieldDescription Permissions => GetField("Permissions");

        public FieldDescription IsDeleted => GetField("IsDeleted");

        public FieldDescription DeletionDate => GetField("DeletionDate");

        public FieldDescription Creation => GetField("Creation");

        public FieldDescription LastModified => GetField("LastModified");

        protected override IDbTableContext GetTableContext(string name)
        {
            if (string.IsNullOrEmpty(name))
                return db?.TableDeclaration<UserAccountGroup>("UserAccountGroups");
            return db?.TableDeclaration<UserAccountGroup>(name);
        }

        protected override IDescription[] GetAllFields()
        {
            return new IDescription[] {
                Id,
                Name,
                Description,
                Permissions,
                IsDeleted,
                DeletionDate,
                Creation,
                LastModified
            };
        }
    }
}
