using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 表达式描述信息解释器。
    /// </summary>
    public class ExpParser : ParserBase
    {
        /// <summary>
        /// 实例化一个表达式描述信息解释器。
        /// </summary>
        /// <param name="adapter">所属的父级适配器。</param>
        public ExpParser(ParserAdapter adapter)
            : base(adapter)
        { }

        /// <summary>
        /// 解释单字符所表示的含义（主要用于运算符处理）。
        /// </summary>
        /// <param name="oper">可能表示运算符的字符信息。</param>
        /// <param name="createParameter">返回是否需要创建参数。</param>
        /// <returns></returns>
        protected virtual string ParsingOperChar(char oper, ref bool createParameter)
        {
            switch (oper)
            {
                case '=':
                    return "=";
                case '≠':
                    return "<>";
                case '<':
                    return "<";
                case '≤':
                    return "<=";
                case '>':
                    return ">";
                case '≥':
                    return ">=";
                case '+':
                case '-':
                case '*':
                case '/':
                case '(':
                case ')':
                    return oper.ToString();
                default:
                    createParameter = true;
                    return oper.ToString();
            }
        }

        /// <summary>
        /// 解释整个表达式信息。
        /// </summary>
        /// <param name="DbParameters">用于缓存在解释过程中可能会产生的参数。</param>
        /// <returns></returns>
        public override string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            ExpDescription expDes = (ExpDescription)this.Description;
            StringBuilder cBuffer = new StringBuilder();
            string paramName = string.Empty;
            foreach (object item in expDes.ExpElements)
            {   // 如果是命令元素描述对象，则调用它相应的解释器。
                if (item is IDescription)
                {
                    IDescription Des = (IDescription)item;
                    Des.DescriptionParserAdapter = expDes.DescriptionParserAdapter;
                    // 如果是字段描述对象，则先将字段名称记录下来（后面的非描述对象可能需要创建参数，这就需要使用到）
                    FieldDescription fdes = Des as FieldDescription;
                    if (!IsNull(fdes))
                        paramName = fdes.FieldName;
                    string buf = Des.GetParser().Parsing(ref DbParameters);
                    // 判断并补空格。
                    if (buf[0] == (char)0x20)
                        cBuffer.Append(buf);
                    else
                        cBuffer.AppendFormat(" {0}", buf);
                }
                else if (item is char) // 如果是单字符，则先分析是否是预定义的运算符，如果是则需要进行相应的转换。
                {
                    bool createParam = false;
                    string buf = ParsingOperChar((char)item, ref createParam);
                    if (createParam && !string.IsNullOrEmpty(paramName))
                    {   // 当单字段并非预定义的运算符时视为值，并为其创建参数。
                        IDbDataParameter p = Adapter.CreateDbParameter(string.Format("u_{0}", paramName), buf);
                        AddDbParameter(ref DbParameters, p);
                        cBuffer.AppendFormat(" {0}", p.ParameterName);
                        paramName = string.Empty; // 参数创建完成后应该清除该记录，否则下一次循环就会一直定沿用。
                    }
                    else
                    {   // 单字符为预定义的运算符时。
                        cBuffer.AppendFormat(" {0}", buf);
                    }
                }
                else // 其他类型的处理
                {
                    if (item.Equals(string.Empty))
                    {   // 空字符串转换。
                        cBuffer.AppendFormat(" ''");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(paramName))
                        {   // 前面没有检测到字段描述信息，不需要创建参数。
                            cBuffer.AppendFormat(" {0}", item);
                        }
                        else
                        {   // 前面的元素检测到字段信息，需要创建参数。
                            IDbDataParameter p = Adapter.CreateDbParameter(string.Format("u_{0}", paramName), item);
                            AddDbParameter(ref DbParameters, p);
                            cBuffer.AppendFormat(" {0}", p.ParameterName);
                            paramName = string.Empty; // 参数创建完成后应该清除该记录，否则下一次循环就会一直定沿用。
                        }
                    }
                }
            }
            return cBuffer.ToString();
        }
    }
}
