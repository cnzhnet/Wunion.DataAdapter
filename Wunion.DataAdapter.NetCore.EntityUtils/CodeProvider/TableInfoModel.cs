using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Wunion.DataAdapter.EntityUtils.CodeProvider
{
    /// <summary>
    /// 表信息的实体模型.
    /// </summary>
    [Serializable()]
    public class TableInfoModel
    {
        private Dictionary<string, object> Data;

        public TableInfoModel()
        {
            Data = new Dictionary<string, object>();

            this.Checked = false;
        }

        /// <summary>
        /// 获取指定属性的值。
        /// </summary>
        /// <typeparam name="T">属性的值类型.</typeparam>
        /// <param name="propertyName">属性名称.</param>
        /// <param name="defaultValue">当属性值为空时，默认值返回此值.</param>
        /// <returns>返回指定属性的值.</returns>
        protected T GetValue<T>(string propertyName, T defaultValue)
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
            // 进行属性值的数据类型转换.
            object propertyValue; // 属性最终的值.
            if (Val == null || Val == DBNull.Value)
            {
                propertyValue = pi.PropertyType.IsValueType ? Activator.CreateInstance(pi.PropertyType) : null;
            }
            else
            {
                if (requiredConvert)
                {
                    if (pi.PropertyType == typeof(string))
                        propertyValue = Val.ToString();
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
        /// 获取或设置是否选中.
        /// </summary>
        public bool Checked
        {
            get { return GetValue<bool>("Checked", false); }
            set { SetValue("Checked", value); }
        }

        /// <summary>
        /// 获取或设置表名.
        /// </summary>
        public string tableName
        {
            get { return GetValue<string>("tableName", string.Empty); }
            set { SetValue("tableName", value); }
        }

        /// <summary>
        /// 获取或设置表的说明(注释信息)
        /// </summary>
        public string tableDescription
        {
            get { return GetValue<string>("tableDescription", string.Empty); }
            set { SetValue("tableDescription", value); }
        }

        /// <summary>
        /// 获取或设置字段信息.
        /// </summary>
        public string paramName
        {
            get { return GetValue<string>("paramName", string.Empty); }
            set { SetValue("paramName", value); }
        }

        /// <summary>
        /// 获取或设置是否为主键.
        /// </summary>
        public bool isPrimary
        {
            get { return GetValue<bool>("isPrimary", false); }
            set { SetValue("isPrimary", value); }
        }

        /// <summary>
        /// 获取或设置是否为自增长字段.
        /// </summary>
        public bool isIdentity
        {
            get { return GetValue<bool>("isIdentity", false); }
            set { SetValue("isIdentity", value); }
        }

        /// <summary>
        /// 获取或设置字段的数据类型 .
        /// </summary>
        public string dbType
        {
            get { return GetValue<string>("dbType", string.Empty); }
            set { SetValue("dbType", value); }
        }

        /// <summary>
        /// 获取或设置字段是否允许为空.
        /// </summary>
        public bool allowNull
        {
            get { return GetValue<bool>("allowNull", false); }
            set { SetValue("allowNull", value); }
        }

        /// <summary>
        /// 获取或设置字段的默认值.
        /// </summary>
        public object defaultValue
        {
            get { return GetValue<object>("defaultValue", null); }
            set { SetValue("defaultValue", value); }
        }

        /// <summary>
        /// 获取设置字段的说明（注释）.
        /// </summary>
        public string paramDescription
        {
            get { return GetValue<string>("paramDescription", string.Empty); }
            set { SetValue("paramDescription", value); }
        }
    }

    /// <summary>
    /// 定义方法以支持对等的比较.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class CommonEqualityComparer<T, V> : IEqualityComparer<T>
    {
        private Func<T, V> keySelector;

        /// <summary>
        /// 创建一个 <see cref="CommonEqualityComparer{T, V}"/> 的对象实例.
        /// </summary>
        /// <param name="keySelector"></param>
        public CommonEqualityComparer(Func<T, V> keySelector)
        {
            this.keySelector = keySelector;
        }

        /// <summary>
        /// 确定指定的对象是否相等.
        /// </summary>
        /// <param name="x">对等比较的左操作数.</param>
        /// <param name="y">对等比较的右操作数.</param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            return EqualityComparer<V>.Default.Equals(keySelector(x), keySelector(y));
        }

        /// <summary>
        /// 返回指定对象的哈希码.
        /// </summary>
        /// <param name="obj">要为其返回散列码的对象.</param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            return EqualityComparer<V>.Default.GetHashCode(keySelector(obj));
        }
    }

    /// <summary>
    /// 为 <see cref="IEnumerable<T>"/> 扩展 Distinct 方法.
    /// </summary>
    public static class DistinctExtensions
    {
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }
    }
}
