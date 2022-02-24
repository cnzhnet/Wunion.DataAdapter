using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;

namespace Wunion.DataAdapter.CodeFirstDemo.Data
{
    /// <summary>
    /// 用户信息的数据模型.
    /// </summary>
    public class UserDataModel
    {
        /// <summary>
        /// 创建一个 <see cref="UserDataModel"/> 模型对象.
        /// </summary>
        public UserDataModel()
        { }

        /// <summary>
        /// 用户账户ID.
        /// </summary>
        public int UID { get; set; }

        /// <summary>
        /// 用户账户名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户账户的状态.
        /// </summary>
        public UserAccountStatus Status { get; set; }

        /// <summary>
        /// 用户账户组.
        /// </summary>
        public List<int> Groups { get; set; }

        /// <summary>
        /// 该用户账户的使用人.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 手机号.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 电子邮件.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 创建日期.
        /// </summary>
        public DateTime? Creation { get; set; }

        /// <summary>
        /// 最后修改日期.
        /// </summary>
        public DateTime? LastModified { get; set; }
    }
}
