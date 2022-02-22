using System;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    /// <summary>
    /// 用于实现授权访问器的接口.
    /// </summary>
    public class AuthorizationAccessor
    {
        /// <summary>
        /// 用户权限信息.
        /// </summary>
        public UserAuthorization Authorization { get; set; }
    }
}
