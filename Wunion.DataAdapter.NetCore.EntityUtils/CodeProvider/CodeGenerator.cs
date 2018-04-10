using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace Wunion.DataAdapter.EntityUtils.CodeProvider
{
    /// <summary>
    /// 用于提供实现代码生成器注释语言支持的接口.
    /// </summary>
    public interface ICodeCommentsLanguage
    {
        /// <summary>
        /// 获取指定键的字符串.
        /// </summary>
        /// <param name="key">资源键的名称.</param>
        /// <returns></returns>
        string GetString(string key);
    }

    /// <summary>
    /// 表示类代码生成器的基础类型.
    /// </summary>
    public abstract class CodeGenerator
    {
        /// <summary>
        /// 命名空间代码.
        /// </summary>
        private CodeNamespace codeNamespace;

        protected CodeGenerator()
        {
            ResetOnAfterWrite = true;
        }

        /// <summary>
        /// 获取或设置当代码写入文件后是否重置生成器状态（默认 true）
        /// </summary>
        public bool ResetOnAfterWrite { get; set; }

        /// <summary>
        /// 获取或设置生成代码时所使用的注释多语言支持.
        /// </summary>
        public ICodeCommentsLanguage LangComments { get; set; }

        /// <summary>
        /// 从多语言支持中获取注释文本内容.
        /// </summary>
        /// <param name="key">注释信息的键.</param>
        /// <param name="defaultVal">当未能从语言资源中获得注释信息时，返回此字符串.</param>
        /// <returns></returns>
        protected string LangCommentString(string key, string defaultVal)
        {
            if (LangComments != null)
            {
                string tmp = LangComments.GetString(key);
                return string.IsNullOrEmpty(tmp) ? defaultVal : tmp;
            }
            return defaultVal;
        }

        /// <summary>
        /// 构建 using 代码片段.
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildCompileUnit()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using Wunion.DataAdapter.EntityUtils;");
            return builder.ToString();
        }

        /// <summary>
        /// 生成类的构造函数 .
        /// </summary>
        /// <param name="classDeclaration">类的生成器对象.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected virtual void AddConstructors(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        { }

        /// <summary>
        /// 生成类的自定义特性信息.
        /// </summary>
        /// <param name="classDeclaration">类的生成器对象.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected virtual void AddCustomAttributes(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        { }

        /// <summary>
        /// 生成类的属性代码.
        /// </summary>
        /// <param name="classDeclaration">类的生成器对象.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected virtual void AddPropertyMembers(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        { }

        /// <summary>
        /// 生成类的方法代码.
        /// </summary>
        /// <param name="classDeclaration">类的生成器对象.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected virtual void AddMethodMembers(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        { }

        /// <summary>
        /// 创建类的代码.
        /// </summary>
        /// <param name="codeClass">用于生成类的一些基本信息.</param>
        /// <returns></returns>
        public CodeTypeDeclaration BuildClass(CodeClassDeclaration codeClass)
        {
            // 创建类代码
            CodeTypeDeclaration classType = new CodeTypeDeclaration();
            classType.Name = codeClass.Name;
            classType.IsClass = true;
            classType.TypeAttributes = System.Reflection.TypeAttributes.Public;
            foreach (string baseType in codeClass.BaseTypes)
                classType.BaseTypes.Add(baseType);

            // 生成实体类的特性.
            AddCustomAttributes(classType, codeClass);
            AddConstructors(classType, codeClass);
            // 生成实体类的属性.
            AddPropertyMembers(classType, codeClass);
            AddMethodMembers(classType, codeClass);
            // 生成类的注释信息.
            if (!string.IsNullOrEmpty(codeClass.CodeComment))
            {
                classType.Comments.Add(new CodeCommentStatement("<summary>", true));
                classType.Comments.Add(new CodeCommentStatement(codeClass.CodeComment, true));
                classType.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
            return classType;
        }

        /// <summary>
        /// 在该生成器中添加一个类.
        /// </summary>
        /// <param name="codeClass">生成类的一些基本信息.</param>
        public void AddClass(CodeClassDeclaration codeClass)
        {
            if (codeNamespace == null)
                codeNamespace = new CodeNamespace(codeClass.Namespace);
            codeNamespace.Types.Add(BuildClass(codeClass));
        }

        /// <summary>
        /// 在该生成器中添加一个类.
        /// </summary>
        /// <param name="classType">由其它生成器生成的类代码.</param>
        public void AddClass(CodeTypeDeclaration classType)
        {
            codeNamespace.Types.Add(classType);
        }

        /// <summary>
        /// 生成并输出类文件.
        /// </summary>
        /// <param name="codeClass">用于生成类的一些基本信息.</param>
        /// <param name="filePath">保存文到该文件.</param>
        /// <param name="overwrite">若源代码文件存在时是否覆盖.</param>
        public void WriteTo(string filePath, bool overwrite = false)
        {
            if (File.Exists(filePath))
            {
                if (overwrite)
                    File.Delete(filePath);
                else
                    return;
            }
            using (StreamWriter codeWriter = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                ICodeGenerator generator = provider.CreateGenerator(codeWriter);
                CodeGeneratorOptions options = new CodeGeneratorOptions
                {
                    BlankLinesBetweenMembers = true,
                    BracingStyle = "C"
                };
                // 生成 using 引用
                generator.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit(BuildCompileUnit()),
                                                      codeWriter, options);
                // 将命名空间中的代码生成到文件.
                generator.GenerateCodeFromNamespace(codeNamespace, codeWriter, options);
            }
            if (ResetOnAfterWrite)
                codeNamespace = null;
        }
    }
}
