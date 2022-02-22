using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Models
{
    /// <summary>
    /// 表示错误信息.
    /// </summary>
    public class ResultMessage
    {
        /// <summary>
        /// 错误代码.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误消息.
        /// </summary>
        public string Message { get; set; }
    }
}
