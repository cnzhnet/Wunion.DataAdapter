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
    /// 数据表上下文对象与实体查询映射的类生成器.
    /// </summary>
    public class EntityContextGenerator : CodeGenerator
    {
        /// <summary>
        /// 创建一个 <see cref="EntityContextGenerator"/> 的对象实例.
        /// </summary>
        public EntityContextGenerator() : base()
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
        /// 生成类的方法代码.
        /// </summary>
        /// <param name="classDeclaration">类的生成器对象.</param>
        /// <param name="codeClass">生成该类的参照信息.</param>
        protected override void AddMethodMembers(CodeTypeDeclaration classDeclaration, CodeClassDeclaration codeClass)
        {
            // List<实体类> Select(Func<查询代理类, object> selector) 方法生成
            CodeMemberMethod codeMethod = new CodeMemberMethod();
            codeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            codeMethod.ReturnType = new CodeTypeReference("List");
            codeMethod.ReturnType.TypeArguments.Add(codeClass.TableName); // Select方法返回的泛型参数.
            codeMethod.Name = "Select";
            CodeTypeReference paramType = new CodeTypeReference("Func");
            paramType.TypeArguments.Add(string.Format("{0}Agent", codeClass.TableName));
            paramType.TypeArguments.Add(typeof(object));
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(paramType, "selector"));
            // 方法代码
            CodeMethodReferenceExpression baseMethod = new CodeMethodReferenceExpression();
            baseMethod.MethodName = "Select";
            baseMethod.TypeArguments.Add(new CodeTypeReference(codeClass.TableName));
            baseMethod.TargetObject = new CodeBaseReferenceExpression();
            CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeMethodInvokeExpression(baseMethod, new CodeSnippetExpression("selector"));
            codeMethod.Statements.Add(returnStatement);
            string strComment = LangCommentString("TableContext.Select(@selector)", "查询实体对应的表，并返回数据集合.");
            codeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            codeMethod.Comments.Add(new CodeCommentStatement(strComment, true));
            strComment = LangCommentString("TableContext.Select@selector", "用于返回查询条件的筛选器.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"selector\">{0}</param>", strComment), true));
            codeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
            strComment = LangCommentString("TableContext.Select:Return", "返回查询结果.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>{0}</returns>", strComment), true));
            classDeclaration.Members.Add(codeMethod);

            // List<实体类> Select(Action<查询代理类, SelectBlock> action) 方法的生成
            codeMethod = new CodeMemberMethod();
            codeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            codeMethod.ReturnType = new CodeTypeReference("List");
            codeMethod.ReturnType.TypeArguments.Add(codeClass.TableName);
            codeMethod.Name = "Select";
            paramType = new CodeTypeReference("Action");
            paramType.TypeArguments.Add(string.Format("{0}Agent", codeClass.TableName));
            paramType.TypeArguments.Add("SelectBlock");
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(paramType, "action"));
            baseMethod = new CodeMethodReferenceExpression();
            baseMethod.MethodName = "Select";
            baseMethod.TypeArguments.Add(new CodeTypeReference(codeClass.TableName));
            baseMethod.TargetObject = new CodeBaseReferenceExpression();
            returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeMethodInvokeExpression(baseMethod, new CodeSnippetExpression("action"));
            codeMethod.Statements.Add(returnStatement);
            codeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            strComment = LangCommentString("TableContext.Select(@action)", "根据复杂的查询条件及分页等行为查询实体对应的表，并返回数据集合.");
            codeMethod.Comments.Add(new CodeCommentStatement(strComment, true));
            strComment = LangCommentString("TableContext.Select@action", "用于指定查询条件、分布信息等.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"action\">{0}</param>", strComment), true));
            codeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
            strComment = LangCommentString("TableContext.Select:Return", "返回查询结果.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>{0}</returns>", strComment), true));
            classDeclaration.Members.Add(codeMethod);

            // void Add(<实体类>, DBTransactionController trans = null) 方法的生成
            codeMethod = new CodeMemberMethod();
            codeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            codeMethod.Name = "Add";
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(codeClass.TableName), "entity"));
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("DBTransactionController"), "trans = null"));
            baseMethod = new CodeMethodReferenceExpression();
            baseMethod.MethodName = "Add";
            baseMethod.TypeArguments.Add(new CodeTypeReference(codeClass.TableName));
            baseMethod.TargetObject = new CodeBaseReferenceExpression();
            codeMethod.Statements.Add(new CodeMethodInvokeExpression(baseMethod, new CodeSnippetExpression("entity"), new CodeSnippetExpression("trans")));
            codeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            strComment = LangCommentString("TableContext.Add(@entity,@trans)", "向数据表中添加一条记录.");
            codeMethod.Comments.Add(new CodeCommentStatement(strComment, true));
            codeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
            strComment = LangCommentString("TableContext.Add@TEntity", "数据表对应的实体类型.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<typeparam name=\"TEntity\">{0}</typeparam>", strComment), true));
            strComment = LangCommentString("TableContext.Add@entity", "实体对象.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"entity\">{0}</param>", strComment), true));
            strComment = LangCommentString("TableContext.Add@trans", "写事务的事务控制器.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"trans\">{0}</param>", strComment), true));
            strComment = LangCommentString("TableContext.Add:Exception", "当新增数据出错时引发该异常.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"Exception\">{0}</exception>", strComment), true));
            classDeclaration.Members.Add(codeMethod);

            // int Update(<实体类>, Func<查询代理类, object[]> func) 方法的生成
            codeMethod = new CodeMemberMethod();
            codeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            codeMethod.ReturnType = new CodeTypeReference(typeof(int));
            codeMethod.Name = "Update";
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(codeClass.TableName), "entity"));
            paramType = new CodeTypeReference("Func");
            paramType.TypeArguments.Add(new CodeTypeReference(string.Format("{0}Agent", codeClass.TableName)));
            paramType.TypeArguments.Add(new CodeTypeReference(typeof(object[])));
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(paramType, "func"));
            baseMethod = new CodeMethodReferenceExpression();
            baseMethod.MethodName = "Update";
            baseMethod.TypeArguments.Add(new CodeTypeReference(codeClass.TableName));
            baseMethod.TargetObject = new CodeBaseReferenceExpression();
            returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeMethodInvokeExpression(baseMethod, new CodeSnippetExpression("entity"), new CodeSnippetExpression("func"));
            codeMethod.Statements.Add(returnStatement);
            codeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            strComment = LangCommentString("TableContext.Update(@entity,@func)", "更新数据表中的记录.");
            codeMethod.Comments.Add(new CodeCommentStatement(strComment, true));
            codeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
            strComment = LangCommentString("TableContext.Update@entity", "实体对象.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"entity\">{0}</param>", strComment), true));
            strComment = LangCommentString("TableContext.Update@func", "用于创建更新条件.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"func\">{0}</param>", strComment), true));
            strComment = LangCommentString("TableContext.Update:ArgumentNullException", "当必备的参数为空时引发此异常.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"ArgumentNullException\">{0}</exception>", strComment), true));
            strComment = LangCommentString("TableContext.Update:Exception", "当执行更新时产生错误时引发此异常.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"Exception\">{0}</exception>", strComment), true));
            strComment = LangCommentString("TableContext.Update:Return", "返回受影响的记录数.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>{0}</returns>", strComment), true));
            classDeclaration.Members.Add(codeMethod);

            // int Update(DBTransactionController, <实体类>, Func<查询代理类, object[]>) 方法的生成.
            codeMethod = new CodeMemberMethod();
            codeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            codeMethod.ReturnType = new CodeTypeReference(typeof(int));
            codeMethod.Name = "Update";
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("DBTransactionController"), "trans"));
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(codeClass.TableName), "entity"));
            paramType = new CodeTypeReference("Func");
            paramType.TypeArguments.Add(new CodeTypeReference(string.Format("{0}Agent", codeClass.TableName)));
            paramType.TypeArguments.Add(new CodeTypeReference(typeof(object[])));
            codeMethod.Parameters.Add(new CodeParameterDeclarationExpression(paramType, "func"));
            baseMethod = new CodeMethodReferenceExpression();
            baseMethod.MethodName = "Update";
            baseMethod.TypeArguments.Add(new CodeTypeReference(codeClass.TableName));
            baseMethod.TargetObject = new CodeBaseReferenceExpression();
            returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeMethodInvokeExpression(baseMethod,
                new CodeSnippetExpression("trans"),
                new CodeSnippetExpression("entity"),
                new CodeSnippetExpression("func"));
            codeMethod.Statements.Add(returnStatement);
            codeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
            strComment = LangCommentString("TableContext.Update(@trans,@entity,@func)", "在事务中更新数据表中的记录.");
            codeMethod.Comments.Add(new CodeCommentStatement(strComment, true));
            codeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
            strComment = LangCommentString("TableContext.Update@trans", "事务控制器.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"trans\">{0}</param>", strComment), true));
            strComment = LangCommentString("TableContext.Update@entity", "实体对象.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"entity\">{0}</param>", strComment), true));
            strComment = LangCommentString("TableContext.Update@func", "用于创建更新条件.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"func\">{0}</param>", strComment), true));
            strComment = LangCommentString("TableContext.Update:ArgumentNullException", "当必备的参数为空时引发此异常.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"ArgumentNullException\">{0}</exception>", strComment), true));
            strComment = LangCommentString("TableContext.Update:Exception", "当执行更新时产生错误时引发此异常.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"Exception\">{0}</exception>", strComment), true));
            strComment = LangCommentString("TableContext.Update:Return", "返回受影响的记录数.");
            codeMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>{0}</returns>", strComment), true));
            classDeclaration.Members.Add(codeMethod);
        }
    }
}
