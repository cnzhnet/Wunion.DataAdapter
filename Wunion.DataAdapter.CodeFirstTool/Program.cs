using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TeleprompterConsole
{
    internal class Program
    {
        static ILanguageProvider Language;

        static void Main(string[] args)
        {
            int argIndex = -1;
            Language = new LanguageProvider(FindLocale(args, out argIndex));
            List<string> arguments = new List<string>(args);
            if (argIndex != -1)
                arguments.RemoveAt(argIndex);
            PrintInformation();
            CommandLineApplication.Create(Language).Run(arguments.ToArray());
        }

        /// <summary>
        /// 从传入的命令行中查找语言环境.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string FindLocale(string[] args, out int index)
        {
            index = -1;
            for (int i = 0; i < args.Length; ++i) 
            {
                if (args[i].ToLower().StartsWith("/locale:"))
                {
                    index = i;
                    return args[i].Replace("/locale:", string.Empty).Trim();
                }

            }
            return "zh-CN"; //默认简体中文.
        }

        private static void PrintInformation()
        {
            Console.WriteLine("=====================================");
            Console.WriteLine($"  {Language.GetString("app-name")}");
            string ver = Assembly.GetEntryAssembly()
                                 .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                 .InformationalVersion
                                 .ToString();
            Console.WriteLine($"  {Language.GetString("version", ver)}");
            Console.WriteLine($"  {Language.GetString("copyright")}");
            Console.Write("-------------------------------------\r\n");
            Console.WriteLine(Language.GetString("use-locale", Language.RegionName));
            Console.Write("\r\n");
        }

        /// <summary>
        /// 获取应用程序的工作路径.
        /// </summary>
        /// <returns></returns>
        internal static string GetBasePath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            /*string basePath = string.Empty;
            using (ProcessModule pm = Process.GetCurrentProcess().MainModule)
                basePath = Path.GetDirectoryName(pm?.FileName);
            return basePath;*/
        }
    }
}
