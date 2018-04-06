using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.CommandProviders
{
    /// <summary>
    /// Comments 命令支持.
    /// </summary>
    public static class CommentsProvider
    {
        /// <summary>
        /// 输出命令的使用方法 .
        /// </summary>
        public static void WriteInstructions()
        {
            Console.WriteLine("\tcomments <save | load> <path>");
        }

        /// <summary>
        /// 执行 comments 命令.
        /// </summary>
        /// <param name="parameters">命令的参数列表.</param>
        /// <param name="service">代码生成器服务对象.</param>
        /// <param name="lang">命令的输出语言环境服务.</param>
        public static void Do(List<string> parameters, GeneratorService service, LanguageService lang)
        {
            if (parameters == null || parameters.Count < 2)
            {
                WriteInstructions();
                return;
            }
            if (service == null)
            {
                Console.WriteLine(lang.GetString("NullCodeService"));
                return;
            }
            try
            {
                string filePath = ParametersPathGetter.MergeOne(parameters, 1);
                switch (parameters[0].ToLower())
                {
                    case "save":
                        service.SaveComments(filePath);
                        break;
                    case "load":
                        service.LoadComments(filePath);
                        break;
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }
        }
    }
}
