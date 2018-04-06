using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.EntityGenerator.CommandProviders
{
    /// <summary>
    /// 命令参数的溶合器.
    /// </summary>
    public static class ParametersPathGetter
    {
        /// <summary>
        /// 将命令的参数理表从指定的位置向后合并为一个字符串.
        /// </summary>
        /// <param name="parameters">命令参数列表.</param>
        /// <param name="start">从该位置开始合并.</param>
        /// <returns></returns>
        public static string MergeOne(List<string> parameters, int start)
        {
            if (parameters.Count <= start)
                return string.Empty;
            StringBuilder buffer = new StringBuilder(parameters[start++]);
            for (; start < parameters.Count; ++start)
                buffer.AppendFormat(" {0}", parameters[start]);
            return buffer.ToString();
        }
    }
}
