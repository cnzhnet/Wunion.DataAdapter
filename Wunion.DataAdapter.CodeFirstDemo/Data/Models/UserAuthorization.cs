using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Models
{
    /// <summary>
    /// 表示用户授权信息.
    /// </summary>
    public class UserAuthorization
    {
        /// <summary>
        /// 租户.
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// 用户账户 ID.
        /// </summary>
        public int UID { get; set; }

        /// <summary>
        /// 发行者.
        /// </summary>
        public string Producer { get; set; }

        /// <summary>
        /// 授权日期.
        /// </summary>
        public DateTime Grant { get; set; }

        /// <summary>
        /// 有效期（以分钟为单位）.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// 权限.
        /// </summary>
        public List<int> Permissions { get; set; }
    }

    /// <summary>
    /// 用于返回授权消息.
    /// </summary>
    public class AuthorizationMessage
    { 
        /// <summary>
        /// 授权生效日期.
        /// </summary>
        public DateTime Grant { get; set; }

        /// <summary>
        /// 有效期（以分钟为单位）.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// 授权令牌.
        /// </summary>
        public string Token { get; set; }
    }
}
