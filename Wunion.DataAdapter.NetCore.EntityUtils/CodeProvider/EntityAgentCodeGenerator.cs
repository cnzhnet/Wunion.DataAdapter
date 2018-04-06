using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Wunion.DataAdapter.EntityUtils.CodeProvider
{
    /// <summary>
    /// 实体代理辅助类生成器.
    /// </summary>
    public class EntityAgentCodeGenerator : CodeGenerator
    {
        /// <summary>
        /// 生成一个 <see cref="EntityAgentCodeGenerator"/> 的对象实例.
        /// </summary>
        public EntityAgentCodeGenerator() : base()
        { }

        /// <summary>
        /// 返回类文件的 using 命名空间引用.
        /// </summary>
        /// <returns></returns>
        protected override string BuildCompileUnit()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using Wunion.DataAdapter.Kernel.DbInterop;");
            builder.AppendLine("using Wunion.DataAdapter.Kernel.CommandBuilders;");
            builder.AppendLine("using Wunion.DataAdapter.EntityUtils;");
            return builder.ToString();
        }

        /// <summary>
        /// 生成类的构造函数 .
        /// </summary>
        /// <param name="classDeclaration">类的生成器对象.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected override void AddConstructors(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            string DefaultComment = LangCommentString("DefaultConstructor", string.Empty);
            if (string.IsNullOrEmpty(DefaultComment))
                DefaultComment = string.Format("创建一个 <see cref=\"{0}.{1}\"/> 的对象实例.", codeClass.Namespace, codeClass.Name);
            else
                DefaultComment = string.Format(DefaultComment, codeClass.Namespace, codeClass.Name);
            constructor.Comments.Add(new CodeCommentStatement(DefaultComment, true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
            classDeclaration.Members.Add(constructor);
        }

        /// <summary>
        /// 生成类的自定义特性信息.
        /// </summary>
        /// <param name="classDeclaration">实体类的对象生成器.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected override void AddCustomAttributes(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        {
            classDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("EntityTable", new CodeAttributeArgument("TableName", new CodePrimitiveExpression(codeClass.TableName))));
        }

        /// <summary>
        /// 生成类的属性代码.
        /// </summary>
        /// <param name="classDeclaration">实体类的对象生成器.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected override void AddPropertyMembers(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        {
            if (codeClass.PropertyMembers == null)
                return;
            if (codeClass.PropertyMembers.Count < 1)
                return;
            CodeMemberProperty codeProperty;
            string PropertyName, codeComment;
            foreach (TableInfoModel tableInfo in codeClass.PropertyMembers)
            {
                if (string.IsNullOrEmpty(tableInfo.dbType))
                    continue;
                PropertyName = tableInfo.paramName;
                codeProperty = new CodeMemberProperty();
                codeProperty.Name = PropertyName;
                codeProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final; // 若无 Final 则导致成员为虚拟的
                CodeTypeReference attrType = new CodeTypeReference("FieldDescription");
                codeProperty.Type = attrType;
                // 设置属性的 getter
                CodeMethodReferenceExpression methodGetValue = new CodeMethodReferenceExpression();
                methodGetValue.MethodName = "GetField";
                CodeMethodInvokeExpression invokeGetValue = new CodeMethodInvokeExpression();
                invokeGetValue.Method = methodGetValue;
                invokeGetValue.Parameters.Add(new CodePrimitiveExpression(PropertyName));
                codeProperty.GetStatements.Add(new CodeMethodReturnStatement(invokeGetValue));
                // 生成属性的注释信息
                codeComment = Convert.ToString(tableInfo.paramDescription);
                if (!string.IsNullOrEmpty(codeComment))
                {
                    codeProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                    codeProperty.Comments.Add(new CodeCommentStatement(codeComment, true));
                    codeProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
                }
                classDeclaration.Members.Add(codeProperty);
            }
        }

        /// <summary>
        /// 生成类的方法代码.
        /// </summary>
        /// <param name="classDeclaration">类的生成器对象.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected override void AddMethodMembers(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        {
            CodeMemberMethod codeMethod = new CodeMemberMethod();
            codeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            codeMethod.ReturnType = new CodeTypeReference("TableMapper");
            codeMethod.Name = "CreateContext";
            CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeSnippetExpression(string.Format("new {0}Context()", codeClass.TableName));
            codeMethod.Statements.Add(returnStatement);
            codeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            string CommentText = LangCommentString("EntityAgent.CreateContext()", "创建该代理类代理的数据表上下文对象.");
            codeMethod.Comments.Add(new CodeCommentStatement(CommentText, true));
            codeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
            CommentText = LangCommentString("EntityAgent.CreateContext:Return", "返回映射的数据表上下文对象.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>{0}</returns>", CommentText), true));
            classDeclaration.Members.Add(codeMethod);
        }
    }
}
