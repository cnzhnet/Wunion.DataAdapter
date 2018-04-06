using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if NET462
using System.Windows.Forms;
using Wunion.DataAdapter.EntityGenerator.Views;
#else
using Wunion.DataAdapter.EntityUtils.CodeProvider;
using Wunion.DataAdapter.EntityGenerator.CommandProviders;
#endif
using Wunion.DataAdapter.EntityGenerator.Services;

namespace Wunion.DataAdapter.EntityGenerator
{
    static class Program
    {
        private static LanguageService language;
#if NET462
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            List<LanguageService> services = LanguageService.GetAvailables();
            if (services.Count > 1)
            {
                using (LauncherForm launcher = new LauncherForm(services))
                {
                    launcher.StartPosition = FormStartPosition.CenterScreen;
                    launcher.ShowDialog();
                    language = launcher.Language;
                }
            }
            else
            {
                language = services[0];
            }
            Application.Run(new MainForm(language));
        }
#else
        private static GeneratorService codeService;
        private static SetCommandProvider.SetCommandOptions Options;

        /// <summary>
        /// 应用程序入口点.
        /// </summary>
        /// <param name="args">程序运行的命令行参数.</param>
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                language = LanguageService.LoadResource(args[0].Trim());
                if (language == null)
                    language = LanguageService.LoadResource("en-US");
            }
            else
            {
                language =  LanguageService.LoadResource("en-US");
            }
            PrintTitle();
            Options = new SetCommandProvider.SetCommandOptions();
            string Command;
            do
            {
                Console.Write(language.GetString("EnterPrompt"));
                Command = Console.ReadLine();
                CommandResponse(Command);
            } while (Command.ToLower() != "exit");
        }

        /// <summary>
        /// 打印工具的标题信息.
        /// </summary>
        static void PrintTitle()
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine(language.GetString("CommandLine_Name"));
            Console.WriteLine(string.Format(language.GetString("VersionDisplay"), typeof(Program).Assembly.GetName().Version.ToString()));
            Console.WriteLine(string.Format(language.GetString("LocaleDisplay"), language.RegionName));
            Console.WriteLine(language.GetString("CopyrightText"));
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine();
        }

        /// <summary>
        /// 打印帮助信息.
        /// </summary>
        static void PrintHelp()
        {
            ConnectCommandProvider.WriteInstructions();
            SetCommandProvider.WriteInstructions();
            CommentsProvider.WriteInstructions();
            BuildCommandProvider.WriteInstructions();
            Console.WriteLine("\tclear");
            Console.WriteLine("\texit");
            Console.WriteLine();
        }

        /// <summary>
        /// 响应输入的命令.
        /// </summary>
        /// <param name="command">命令.</param>
        static void CommandResponse(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            string commandName = string.Empty;
            List<string> parameters = CommandParse(command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), out commandName);
            switch (commandName.ToLower())
            {
                case "clear":
                    Console.Clear();
                    PrintTitle();
                    break;
                case "exit":
                    break;
                case "connect":
                    codeService = ConnectCommandProvider.Do(parameters, language);
                    break;
                case "set":
                    SetCommandProvider.Do(parameters, Options, language);
                    break;
                case "comments":
                    CommentsProvider.Do(parameters, codeService, language);
                    break;
                case "build":
                    BuildCommandProvider.Do(parameters, codeService, Options, language);
                    break;
                case "/h":
                case "-h":
                case "--h":
                case "help":
                    PrintHelp();
                    break;
                default:
                    Console.WriteLine(string.Format(language.GetString("CommandNotFound"), commandName));
                    break;
            }
        }

        /// <summary>
        /// 解读命令及其参数.
        /// </summary>
        /// <param name="args">输入的命令参数.</param>
        /// <param name="commandName">命令名称.</param>
        /// <returns></returns>
        static List<string> CommandParse(string[] args, out string commandName)
        {
            commandName = null;
            if (args == null || args.Length < 1)
                return null;
            List<string> parameters = new List<string>();
            commandName = args[0].Trim();
            for (int i = 1; i < args.Length; ++i)
                parameters.Add(args[i].Trim());
            return parameters;
        }
#endif
    }
}
