using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CommandParser
{
    /// <summary>
    /// 解释器的基础对象类型。
    /// </summary>
    public class ParserBase
    {
        protected IDescription _Description;
        protected ParserAdapter _Adapter;

        /// <summary>
        /// 获取或设置该解释器要解释的描述对象。
        /// </summary>
        public IDescription Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary>
        /// 表示命令元素的防保留字冲突左括符（不同的数据库可能不一样，可由子类重写返回其相应符号）。
        /// </summary>
        public string ElemIdentifierL
        {
            get { return Adapter.ElemIdentifierL; }
        }

        /// <summary>
        /// 表示命令元素的防保留字冲突右括符（不同的数据库可能不一样，可由子类重写返回其相应符号）。
        /// </summary>
        public string ElemIdentifierR
        {
            get { return Adapter.ElemIdentifierR; }
        }

        /// <summary>
        /// 创建一个命令元素解释器实例。
        /// </summary>
        /// <param name="adapter">其父级管理适配器。</param>
        protected ParserBase(ParserAdapter adapter)
        {
            _Adapter = adapter;
        }

        /// <summary>
        /// 获取其父级适配器。
        /// </summary>
        public ParserAdapter Adapter
        {
            get { return _Adapter; }
        }

        /// <summary>
        /// 解释 Description 对象。
        /// </summary>
        /// <param name="DbParameters">在解释过程中可能会产生的 DbParameter 参数。</param>
        /// <returns></returns>
        public virtual string Parsing(ref List<IDbDataParameter> DbParameters)
        {
            return null;
        }

        /// <summary>
        /// 向 DbParameters 参数列表中添加一个参数（用该方法添加参数会自动检测重名的参数并为此参数重新命名）。
        /// </summary>
        /// <param name="DbParameters">在解释过程中可能会产生的 DbParameter 参数。</param>
        /// <param name="Parameter">要添加的新参数。</param>
        /// <returns></returns>
        protected void AddDbParameter(ref List<IDbDataParameter> DbParameters, IDbDataParameter Parameter)
        {
            int count = 0;
            string buf;
            Regex Reg = new Regex(@"\d+$");
            foreach (IDbDataParameter p in DbParameters)
            {
                Match m = Reg.Match(p.ParameterName);
                if (m != null && !string.IsNullOrEmpty(m.Value))
                    buf = p.ParameterName.Replace(m.Value, string.Empty);
                else
                    buf = p.ParameterName;
                if (buf == Parameter.ParameterName)
                    count++;
            }
            if (count > 0)
                Parameter.ParameterName = string.Format("{0}{1}", Parameter.ParameterName, count);
            DbParameters.Add(Parameter);
        }

        /// <summary>
        /// 判断指定的对象是否为空。
        /// </summary>
        /// <param name="value">要判断的对象。</param>
        /// <returns></returns>
        public static bool IsNull(object value)
        {
            return object.ReferenceEquals(value, null);
        }
    }
}
