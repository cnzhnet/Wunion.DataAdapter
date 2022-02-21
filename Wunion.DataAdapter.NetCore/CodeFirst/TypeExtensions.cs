using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于扩展 <see cref="Type"/> 对象实例的方法.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取此类型的公共非静态属性（搜索基础）.
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static PropertyInfo[] PublicInstanceProperties(this Type type)
        {
            PropertyInfo[] properties;
            do
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (properties != null && properties.Length > 0)
                    break;
                type = type.BaseType;
            } while (type != null);
            return properties;
        }
    }
}
