using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator.CommandProviders
{
    /// <summary>
    /// build 命令支持.
    /// </summary>
    public static class BuildCommandProvider
    {
        /// <summary>
        /// 输出命令的使用方法 .
        /// </summary>
        public static void WriteInstructions()
        {
            Console.WriteLine("\tbuild <all | -e | -a>");
            Console.WriteLine("\t\t-e\tentity class.");
            Console.WriteLine("\t\t-a\tagent class.");
        }

        /// <summary>
        /// 执行 build 命令.
        /// </summary>
        /// <param name="parameters">参数列表.</param>
        /// <param name="service">代码生成器服务.</param>
        /// <param name="options">代码生成选项.</param>
        /// <param name="lang">命令输出的语言环境服务.</param>
        public static void Do(List<string> parameters, GeneratorService service, SetCommandProvider.SetCommandOptions options, LanguageService lang)
        {
            if (parameters == null || parameters.Count != 1)
            {
                WriteInstructions();
                return;
            }
            if (service == null)
            {
                Console.WriteLine(lang.GetString("NullCodeService"));
                return;
            }
            if (string.IsNullOrEmpty(options.CodeNamespace))
            {
                Console.WriteLine(lang.GetString("NullCodeNamespace"));
                return;
            }
            if (string.IsNullOrEmpty(options.OutputDir))
            {
                Console.WriteLine(lang.GetString("NullOutputDir"));
                return;
            }
            if (service.AllTables == null || service.AllTables.Count < 1)
            {
                Console.WriteLine(lang.GetString("NoTableAvailable"));
                return;
            }
            try
            {
                service.Language = lang;
                service.OutputDir = options.OutputDir;
                switch (parameters[0].ToLower())
                {
                    case "all":
                        service.Pattern = GeneratorService.BuildPattern.BuildAll;
                        service.BuildTo(options.CodeNamespace);
                        Console.WriteLine(lang.GetString("BuildingCompleted"));
                        break;
                    case "-e":
                        service.Pattern = GeneratorService.BuildPattern.BuildEntity;
                        service.BuildTo(options.CodeNamespace);
                        Console.WriteLine(lang.GetString("BuildingCompleted"));
                        break;
                    case "-a":
                        service.Pattern = GeneratorService.BuildPattern.BuildAgent;
                        service.BuildTo(options.CodeNamespace);
                        Console.WriteLine(lang.GetString("BuildingCompleted"));
                        break;
                    default:
                        Console.WriteLine(lang.GetString("DoNothing"));
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
