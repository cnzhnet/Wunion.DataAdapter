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
    /// 表示实体类的代码生成器.
    /// </summary>
    public class EntityCodeGenerator : CodeGenerator
    {
        /// <summary>
        /// 创建一个 <see cref="EntityCodeGenerator"/> 的对象实例.
        /// </summary>
        public EntityCodeGenerator() : base() { }

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
            classDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("Serializable")));
            //classDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("EntityTable", new CodeAttributeArgument("TableName", new CodePrimitiveExpression(codeClass.Name))));
        }

        /// <summary>
        /// 生成属性成员的自定义特性信息.
        /// </summary>
        /// <param name="codeProperty">该此属性成员添加自定义特性.</param>
        private void AddPropertyCustomAttributes(CodeMemberProperty codeProperty, TableInfoModel tableInfo, Type propType)
        {
            CodeAttributeDeclaration codeAttribute = new CodeAttributeDeclaration("EntityProperty");
            bool allowNull = Convert.ToBoolean(tableInfo.allowNull);
            codeAttribute.Arguments.Add(new CodeAttributeArgument("AllowNull", new CodePrimitiveExpression(allowNull)));
            if (Convert.ToBoolean(tableInfo.isPrimary))
                codeAttribute.Arguments.Add(new CodeAttributeArgument("PrimaryKey", new CodePrimitiveExpression(true)));
            if (Convert.ToBoolean(tableInfo.isIdentity))
                codeAttribute.Arguments.Add(new CodeAttributeArgument("IsIdentity", new CodePrimitiveExpression(true)));
            if (propType != typeof(DateTime))
            {
                object defaultValue = null;
                if (string.IsNullOrWhiteSpace(Convert.ToString(tableInfo.defaultValue)))
                {
                    if (propType.IsArray)
                        defaultValue = null;
                    else
                        defaultValue = (propType == typeof(string)) ? null : System.Activator.CreateInstance(propType);
                }
                else
                {
                    try
                    {
                        defaultValue = Convert.ChangeType(tableInfo.defaultValue, propType);
                    }
                    catch
                    {
                        if (propType.IsArray)
                            defaultValue = null;
                        else
                            defaultValue = (propType == typeof(string)) ? null : System.Activator.CreateInstance(propType);
                    }
                }
                codeAttribute.Arguments.Add(new CodeAttributeArgument("DefaultValue", new CodePrimitiveExpression(defaultValue)));
            }
            codeProperty.CustomAttributes.Add(codeAttribute);
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
            Type propType;
            foreach (TableInfoModel tableInfo in codeClass.PropertyMembers)
            {
                if (string.IsNullOrEmpty(tableInfo.dbType))
                    continue;
                PropertyName = tableInfo.paramName;
                codeProperty = new CodeMemberProperty();
                codeProperty.Name = PropertyName;
                codeProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final; // 若无 Final 则导致成员为虚拟的
                propType = codeClass.GetTypeFromDbTpye(tableInfo.dbType);
                if (propType == null)
                    throw new Exception(string.Format("未能映射属性 {0} 的类型：{1}", PropertyName, tableInfo.dbType));
                CodeTypeReference attrType = new CodeTypeReference(propType);
                codeProperty.Type = attrType;
                // 设置属性的 getter
                CodeMethodReferenceExpression methodGetValue = new CodeMethodReferenceExpression();
                methodGetValue.MethodName = "GetValue";
                methodGetValue.TypeArguments.Add(attrType);
                CodeMethodInvokeExpression invokeGetValue = new CodeMethodInvokeExpression();
                invokeGetValue.Method = methodGetValue;
                invokeGetValue.Parameters.Add(new CodePrimitiveExpression(PropertyName));
                codeProperty.GetStatements.Add(new CodeMethodReturnStatement(invokeGetValue));
                // 设置属性的 setter
                CodeMethodReferenceExpression methodSetValue = new CodeMethodReferenceExpression();
                methodSetValue.MethodName = "SetValue";
                CodeMethodInvokeExpression invokeSetValue = new CodeMethodInvokeExpression();
                invokeSetValue.Method = methodSetValue;
                invokeSetValue.Parameters.AddRange(new CodeExpression[] {
                    new CodePrimitiveExpression(PropertyName),
                    new CodePropertySetValueReferenceExpression()
                });
                codeProperty.SetStatements.Add(invokeSetValue);
                AddPropertyCustomAttributes(codeProperty, tableInfo, codeClass.GetTypeFromDbTpye(tableInfo.dbType)); // 添加属性的自定义标签特性.
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
    }
}
