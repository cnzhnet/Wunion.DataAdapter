using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    /// <summary>
    /// 应用程序的所有权限定义.
    /// </summary>
    public static class SystemPermissions
    {
        /// <summary>
        /// 调用数据保护 api 的权限.
        /// </summary>
        public const int DATA_PROTECTION_USE = 100;

        /// <summary>
        /// 创建数据保护服务密钥的权限.
        /// </summary>
        public const int DATA_PROTECTION_CK = 101;

        /// <summary>
        /// 读取用户账户的权限.
        /// </summary>
        public const int USER_ACCOUNT_RD = 200;

        /// <summary>
        /// 创建用户账户的权限.
        /// </summary>
        public const int USER_ACCOUNT_CT = 201;

        /// <summary>
        /// 修改用户账户的权限.
        /// </summary>
        public const int USER_ACCOUNT_CH = 202;

        /// <summary>
        /// 删除用户账户的权限.
        /// </summary>
        public const int USER_ACCOUNT_RM = 203;
    }
}
