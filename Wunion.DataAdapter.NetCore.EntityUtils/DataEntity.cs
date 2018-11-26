using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 数据实体基础对象类型.
    /// </summary>
    [Serializable]
    public abstract class DataEntity
    {
        private Dictionary<string, object> Data;

        /// <summary>
        /// 创建一个 <see cref="T:Wunion.Azure.Budget.Core.DataEntity"/> 的对象实例.
        /// </summary>
        protected DataEntity()
        {
            Data = new Dictionary<string, object>();
        }

        /// <summary>
        /// 获取实体中指定属性的值.
        /// </summary>
        /// <param name="propertyName">属性名称.</param>
        /// <typeparam name="T">属性值的数据类型.</typeparam>
        /// <returns>返回指定属性的值.</returns>
        public T GetValue<T>(string propertyName)
        {
            // 首先获得属性成员信息，以便进行属性值的数据有效性验证.
            Type typeMe = this.GetType();
            PropertyInfo pi = typeMe.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (pi == null)
                throw (new Exception(string.Format("未找到实体的属性：{0}", propertyName)));
            // 获得得用于验证属性值的有效性的特性信息.
            EntityPropertyAttribute attribute = pi.GetCustomAttribute(typeof(EntityPropertyAttribute)) as EntityPropertyAttribute;
            if (attribute == null) // 若属性未指定相关的特征，则创建默认的数据有效性特征.
                attribute = new EntityPropertyAttribute();
            return GetValue<T>(propertyName, attribute.DefaultValue);
        }

        /// <summary>
        /// 获取指定属性的值。
        /// </summary>
        /// <typeparam name="T">属性的值类型.</typeparam>
        /// <param name="propertyName">属性名称.</param>
        /// <param name="defaultValue">当属性值为空时，默认值返回此值.</param>
        /// <returns>返回指定属性的值.</returns>
        protected T GetValue<T>(string propertyName, object defaultValue)
        {
            Type destType = typeof(T);
            object Val = null;
            if (Data.ContainsKey(propertyName))
                Val = Data[propertyName];
            if (Val == null || Val == DBNull.Value)
            {
                if (destType.IsValueType)
                    return (defaultValue == null) ? (T)(Activator.CreateInstance(destType)) : (T)defaultValue;
                if (destType == typeof(string))
                    return (defaultValue == null) ? (T)((object)(string.Empty)) : (T)defaultValue;
                else
                    return (defaultValue == null) ? default(T) : (T)defaultValue;
            }
            return (T)(Convert.ChangeType(Val, destType));
        }

        /// <summary>
        /// 设置指定字段的值.
        /// </summary>
        /// <param name="propertyName">属性名称.</param>
        /// <param name="Val">属性值.</param>
        /// <param name="requiredConvert">若需进行数据类型转换则为 <c>true</c>，否则应为 <c>false</c>.</param>
        public void SetValue(string propertyName, object Val, bool requiredConvert = false)
        {
            // 首先获得属性成员信息，以便进行属性值的数据有效性验证.
            Type typeMe = this.GetType();
            PropertyInfo pi = typeMe.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (pi == null)
                return;
            // 获得得用于验证属性值的有效性的特性信息.
            EntityPropertyAttribute attribute = pi.GetCustomAttribute(typeof(EntityPropertyAttribute)) as EntityPropertyAttribute;
            if (attribute == null) // 若属性未指定相关的特征，则创建默认的数据有效性特征.
                attribute = new EntityPropertyAttribute();
            // 进行属性值的数据类型转换.
            object propertyValue; // 属性最终的值.
            if (Val == null || Val == DBNull.Value)
            {
                if (!(attribute.AllowNull))
                    throw new NotSupportedException(string.Format("属性 {0} 不允许空值.", propertyName));
                propertyValue = attribute.DefaultValue;
                if (propertyValue == null)
                    propertyValue = pi.PropertyType.IsValueType ? Activator.CreateInstance(pi.PropertyType) : null;
            }
            else
            {
                if (requiredConvert)
                {
                    if (pi.PropertyType == typeof(string))
                        propertyValue = Val.ToString();
                    else if (pi.PropertyType == typeof(object))
                        propertyValue = Val;
                    else
                        propertyValue = Convert.ChangeType(Val, pi.PropertyType);
                }
                else
                {
                    propertyValue = Val;
                }
            }
            // 将属性值存入字典中.
            if (Data.ContainsKey(propertyName))
                Data[propertyName] = propertyValue;
            else
                Data.Add(propertyName, propertyValue);
        }

        /// <summary>
        /// 将实体输出为字典.
        /// </summary>
        /// <param name="withOuts">不包含在输出字典中的实体属性名称.</param>
        /// <returns>返回一个包含实体数据的字典.</returns>
        public virtual Dictionary<string, object> ToDictionary(params string[] withOuts)
        {
            if (withOuts == null || withOuts.Length < 1)
                return Data;
            Dictionary<string, object> Result = new Dictionary<string, object>();
            List<string> withOutList = new List<string>(withOuts);
            foreach (KeyValuePair<string, object> item in Data)
            {
                if (withOutList.Contains(item.Key))
                    continue;
                Result.Add(item.Key, item.Value);
            }
            return Result;
        }
    }
}
