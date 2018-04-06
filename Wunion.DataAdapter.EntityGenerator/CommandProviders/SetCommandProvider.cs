using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.CommandProviders
{
    /// <summary>
    /// set 命令的支持.
    /// </summary>
    public static class SetCommandProvider
    {
        /// <summary>
        /// 执行 set 命令.
        /// </summary>
        /// <param name="parameters">参数列表.</param>
        /// <param name="options">要设置的参数对象.</param>
        /// <param name="lang">set 命令输出的语言环境服务.</param>
        public static void Do(List<string> parameters, SetCommandOptions options, LanguageService lang)
        {
            if (parameters == null || parameters.Count != 2)
            {
                WriteInstructions();
                return;
            }
            switch (parameters[0].ToLower())
            {
                case "namespace":
                    options.CodeNamespace = parameters[1];
                    Console.WriteLine(string.Format(lang.GetString("AlreadySetOption"), parameters[0]));
                    break;
                case "output":
                    options.OutputDir = ParametersPathGetter.MergeOne(parameters, 1);
                    Console.WriteLine(string.Format(lang.GetString("AlreadySetOption"), parameters[0]));
                    break;
                default:
                    Console.WriteLine(lang.GetString("DoNothing"));
                    break;
            }
        }

        /// <summary>
        /// 输出命令的使用方法 .
        /// </summary>
        public static void WriteInstructions()
        {
            Console.WriteLine("\tset <namespace | output> <option value>");
        }

        /// <summary>
        /// 表示 set 命令要设置的选项.
        /// </summary>
        public class SetCommandOptions
        {
            public string CodeNamespace { get; set; }

            public string OutputDir { get; set; }

            /// <summary>
            /// 创建一个 <see cref="SetCommandOptions"/> 对象.
            /// </summary>
            public SetCommandOptions()
            { }
        }
    }
}
