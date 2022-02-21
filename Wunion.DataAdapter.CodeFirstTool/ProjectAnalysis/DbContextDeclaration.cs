using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CodeFirst;

namespace TeleprompterConsole.ProjectAnalysis
{
    /// <summary>
    /// 表示数据库架构的分析结果描述.
    /// </summary>
    public class DbContextDeclaration
    {
        internal DbContextDeclaration()
        { }

        /// <summary>
        /// 定义数据库上下文的目标程序集.
        /// </summary>
        public Assembly TargetAssembly { get; set; }

        /// <summary>
        /// 目标程序集定义的数据库上下文对象实例.
        /// </summary>
        public DbContext DBC { get; set; }

        /// <summary>
        /// 目标数据库及代码的生成配置.
        /// </summary>
        public GeneratingOptions Generating { get; set; }

        /// <summary>
        /// 数据库中的表声明分析结果.
        /// </summary>
        public DbTableDeclaration[] TableDeclarations { get; set; }
    }

    /// <summary>
    /// 表示数据库表声明分析结果.
    /// </summary>
    public class DbTableDeclaration
    {
        /// <summary>
        /// 创建一个 <see cref="DbTableDeclaration"/> 的对象实例.
        /// </summary>
        /// <param name="tblName">表名称.</param>
        /// <param name="_enityType">定义表结构的实体类型.</param>
        internal DbTableDeclaration(string tblName, Type _enityType)
        {
            Name = tblName;
            EntityType = _enityType;
            Order = 0;
        }

        /// <summary>
        /// 表名称.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 定义表结构的实体类型
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// 生成顺序.
        /// </summary>
        public int Order { get; set; }
    }
}
