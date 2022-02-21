using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Wunion.DataAdapter.Kernel;

namespace TeleprompterConsole.ProjectAnalysis
{
    /// <summary>
    /// 用于实现目标程序集的代码分析的接口.
    /// </summary>
    public interface ICodeAnalyzer
    {
        /// <summary>
        /// 获取或设置语言环境.
        /// </summary>
        ILanguageProvider Langauge { get; set; }

        /// <summary>
        /// 用于输出目标.
        /// </summary>
        Action<string> WriteLog { get; set; }

        /// <summary>
        /// 数据库引擎对象.
        /// </summary>
        DataEngine DbEngine { get; set; }

        /// <summary>
        /// 运行代码分析器.
        /// </summary>
        /// <param name="arg">实现代码分析可能需要的参数.</param>
        void Run(object arg = null);
    }

    /// <summary>
    /// 目标程序集代码分析器的基础类型.
    /// </summary>
    public abstract class CodeAnalyzer : ICodeAnalyzer, IDisposable
    {
        /// <summary>
        /// 创建一个 <see cref="CodeAnalyzer"/> 的对象实例.
        /// </summary>
        protected CodeAnalyzer()
        { }

        /// <summary>
        /// 获取或设置语言环境.
        /// </summary>
        public ILanguageProvider Langauge { get; set; }

        /// <summary>
        /// 用于输出目标.
        /// </summary>
        public Action<string> WriteLog { get; set; }

        /// <summary>
        /// 数据库引擎对象.
        /// </summary>
        public DataEngine DbEngine { get; set; }

        /// <summary>
        /// 运行代码分析器.
        /// </summary>
        /// <param name="arg">实现代码分析可能需要的参数.</param>
        public abstract void Run(object arg = null);

        /// <summary>
        /// 加载目标程序集，并创建其代码分析器.
        /// </summary>
        /// <param name="name">目标程序集的名称.</param>
        /// <param name="targetDir">目标程序集所在的路径.</param>
        /// <param name="lang">语言环境.</param>
        /// <returns></returns>
        public static AssemblyCodeAnalyzer LoadAssembly(string name, string targetDir, ILanguageProvider lang)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(targetDir))
                throw new ArgumentNullException(nameof(targetDir));

            string filePath = Path.Combine(targetDir, name);
            if (!File.Exists(filePath))
                throw new DllNotFoundException(lang.GetString("dll_not_found_exception", filePath));

            return new AssemblyCodeAnalyzer(name, targetDir, lang);
        }

        #region IDisposable成员

        /// <summary>
        /// 释放对象占用的资源.
        /// </summary>
        /// <param name="disposing">手动调用则为 true，由 GC 对象终结器调用时则应为 false. </param>
        protected virtual void Dispose(bool disposing)
        { }

        /// <summary>
        /// 释放对象占用的资源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 对象终结器（析构函数）.
        /// </summary>
        ~CodeAnalyzer()
        {
            Dispose(false);
        }

        #endregion
    }
}
