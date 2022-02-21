using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TeleprompterConsole.ProjectAnalysis;

namespace TeleprompterConsole.Generating
{
    /// <summary>
    /// 用于实现生成器的接口.
    /// </summary>
    public interface IGenerator
    { 
        /// <summary>
        /// 获取语言环境提供程序.
        /// </summary>
        ILanguageProvider Language { get; }
        /// <summary>
        /// 目标项目信息.
        /// </summary>
        ProjectInfo TargetProject { get; set; }

        /// <summary>
        /// 目标程序编译的程序集.
        /// </summary>
        Assembly TargetAssembly { get; set; }

        /// <summary>
        /// 用于输出日志信息.
        /// </summary>
        Action<string> WriteLog { get; set; }

        /// <summary>
        /// 运行任务.
        /// </summary>
        /// <param name="arg">运行生成任务需要的参数.</param>
        void Run(DbContextDeclaration arg);
    }

    /// <summary>
    /// 实现生成器的基础类型.
    /// </summary>
    public abstract class Generator : IGenerator
    {
        /// <summary>
        /// 创建一个 <see cref="Generator"/> 的对象实例.
        /// </summary>
        /// <param name="projRoot">目标项目的根目录.</param>
        /// <param name="lang">语言环境提供程序.</param>
        protected Generator(string projRoot, ILanguageProvider lang)
        {
            ProjectRoot = projRoot;
            Language = lang;
        }

        /// <summary>
        /// 获取或设置语言环境.
        /// </summary>
        public ILanguageProvider Language { get; private set; }

        /// <summary>
        /// 用于输出日志信息.
        /// </summary>
        public Action<string> WriteLog { get; set; }

        /// <summary>
        /// 表示项目的根目录.
        /// </summary>
        protected string ProjectRoot { get; set; }

        /// <summary>
        /// 目标项目信息.
        /// </summary>
        public ProjectInfo TargetProject { get; set; }

        /// <summary>
        /// 目标程序编译的程序集.
        /// </summary>
        public Assembly TargetAssembly { get; set; }

        /// <summary>
        /// 运行任务.
        /// </summary>
        /// <param name="arg">运行生成任务需要的参数.</param>
        public abstract void Run(DbContextDeclaration arg);
    }
}
