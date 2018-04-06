using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.NetCore.Demo.Models
{
    /// <summary>
    /// 定义状态码的静态类型。
    /// </summary>
    public static class WebAPIStatus
    {
        /// <summary>
        /// 表示没有数据的状态。
        /// </summary>
        public const int STATE_NO_DATA = -1;
        /// <summary>
        /// 表示成功的状态。
        /// </summary>
        public const int STATE_OK = 0x0;
        /// <summary>
        /// 表示统一的失败状态码。
        /// </summary>
        public const int STATE_FAIL = 0x1;
        /// <summary>
        /// 表示数据库交互错误（由数据库引擎导致的异常）。
        /// </summary>
        public const int DATABASE_EXCEPTION = 21;


        /// <summary>
        /// 表示没有登入。
        /// </summary>
        public const int NO_LOGIN = 42;
        /// <summary>
        /// 表示登入失败。
        /// </summary>
        public const int LOGIN_FAIL = 43;
        /// <summary>
        /// 表示没有权限。
        /// </summary>
        public const int PERMISSION_DENIED = 44;
        /// <summary>
        /// 表示服务端代码错误。
        /// </summary>
        public const int INTERNAL_SERVER_ERROR = 500;
        /// <summary>
        /// 表示没有实现或不支持。
        /// </summary>
        public const int NOT_SUPPORTED = 1400;
    }
}
