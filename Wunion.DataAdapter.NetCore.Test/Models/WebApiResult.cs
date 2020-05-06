using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.NetCore.Test.Models
{
    /// <summary>
    /// WebAPI 返回的结果消息对象类型。
    /// </summary>
    /// <typeparam name="TData"> data 属性的目标数据类型名称.</typeparam>
    [Serializable()]
    public class WebApiResult<TData>
    {
        /// <summary>
        /// 创建一个 <see cref="ResultMessage"/> 的对象类型.
        /// </summary>
        public WebApiResult()
        { }

        /// <summary>
        /// 获取或设置结果标识代码。
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 获取或设置返回结果的消息（应仅在产生错误时设置此值）。
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 返回数据的格式类型（客户端根据该信息来对 Data 进行解码）。
        /// </summary>
        public string dataFormat { get; set; }

        /// <summary>
        /// 返回的数据内容。
        /// </summary>
        public TData data { get; set; }
    }

    /// <summary>
    /// WebAPI 返回结果代码的定义。
    /// </summary>
    public static class ResultCode
    {
        /// <summary>
        /// 表示成功的结果代码。
        /// </summary>
        public const int STATE_OK = 0x0;
        /// <summary>
        /// 表示失败的结果代码。
        /// </summary>
        public const int STATE_FAIL = -1;
        /// <summary>
        /// 表示缺少参数的错误代码
        /// </summary>
        public const int MISSING_ARGUMENT = 21;
        /// <summary>
        /// 表示参数无效的错误
        /// </summary>
        public const int ARGUMENT_IS_INVALID = 22;
        /// <summary>
        /// 表示没有权限。
        /// </summary>
        public const int PERMISSION_DENIED = 44;
        /// <summary>
        /// 表示拒绝服务。
        /// </summary>
        public const int DENIAL_OF_SERVICE = 45;
        /// <summary>
        /// 表示许可证无效。
        /// </summary>
        public const int LICENSE_IS_INVALID = 46;
        /// <summary>
        /// 表示未授予许可证.
        /// </summary>
        public const int NO_LICENSE_GRANTED = 47;
        /// <summary>
        /// 表示证可证限制的 IP 错误.
        /// </summary>
        public const int LICENSE_BAD_IPADDRESS = 48;
        /// <summary>
        /// 表示无效的 Token 数据.
        /// </summary>
        public const int TOKEN_IS_INVALID = 49;
        /// <summary>
        /// 表示服务端代码异常。
        /// </summary>
        public const int SERVER_ERROR = 500;
    }
}
