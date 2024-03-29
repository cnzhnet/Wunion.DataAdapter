﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Linq;
using Wunion.DataAdapter.Kernel.DataCollection;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于扩展 <see cref="ID"/>
    /// </summary>
    internal static class DataReaderExtensions
    {
        /// <summary>
        /// 将指定的指进行类型转换并赋值到实体对象的属性.
        /// </summary>
        /// <param name="db">数据库引擎对象.</param>
        /// <param name="attr">实体对象属性的数据库映射标签特性.</param>
        /// <param name="pi">实体对象的属性信息.</param>
        /// <param name="propertyType">该属性的类型.</param>
        /// <param name="value">要转换并设置到该属性的值.</param>
        /// <param name="entity">实体的对象实体.</param>
        private static void SetEntityProperty(DataEngine db, TableFieldAttribute attr, PropertyInfo pi, Type propertyType, object value, object entity)
        {
            IDbValueConverter converter = null;
            if (attr.ValueConverter == null)
                converter = db.GetValueConverter(pi.PropertyType);
            else
                converter = Activator.CreateInstance(attr.ValueConverter) as IDbValueConverter;
            if (converter == null)
            {
                propertyType = Nullable.GetUnderlyingType(pi.PropertyType) ?? propertyType;
                if (propertyType == null)
                {
                    if (value == null || value == DBNull.Value)
                        return;
                    pi.SetValue(entity, Convert.ChangeType(value, pi.PropertyType));
                }
                else
                {
                    if (value != null && value != DBNull.Value)
                        pi.SetValue(entity, Convert.ChangeType(value, propertyType));
                }
            }
            else
            {
                pi.SetValue(entity, converter.Parse(value));
            }
        }

        /// <summary>
        /// 将数据构建为指定的实体列表.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="reader"><see cref="IDataReader"/> 数据读取器.</param>
        /// <param name="db">数据库引擎对象.</param>
        /// <returns></returns>
        internal static List<TEntity> ToEntityList<TEntity>(this IDataReader reader, DataEngine db) where TEntity : class, new()
        {
            // 获取实体的表字段映射信息.
            Type t = typeof(TEntity);
            List<DbConversionMapping> mappings;
            if (!db.DbConversionPool.TryGetValue(t, out mappings))
            {
                mappings = DbConversionMapping.Get(t);
                if (mappings.Count > 0)
                    db.DbConversionPool.TryAdd(t, mappings);
            }
            if (mappings.Count < 1)
                throw new NotSupportedException(string.Format("The given object does not support this operation, you need to mark the attribute with TableFieldAttribute in the type of that object."));
            // 开始读取数据.
            List<TEntity> output = new List<TEntity>();
            string fieldName = string.Empty;
            object fieldValue = null;
            DbConversionMapping mp;
            TEntity entity = null;
            int i = 0;
            while (reader.Read())
            {
                entity = new TEntity();
                for (i = 0; i < reader.FieldCount; ++i)
                {
                    fieldName = fieldName = reader.GetName(i);
                    mp = mappings.Where(p => p.Attribute.Name == fieldName || p.Property.Name == fieldName).FirstOrDefault();
                    if (mp == null)
                        continue;
                    fieldValue = reader.GetValue(i);
                    SetEntityProperty(db, mp.Attribute, mp.Property, mp.Property.PropertyType, fieldValue, entity);
                }
                output.Add(entity);
            }
            return output;
        }

        /// <summary>
        /// 将数据构建为指定的对象集合.
        /// </summary>
        /// <typeparam name="T">目标对象类型.</typeparam>
        /// <param name="reader"></param>
        /// <param name="db">数据引擎对象.</param>
        /// <returns></returns>
        internal static List<T> ToList<T>(this IDataReader reader, DataEngine db) where T : class, new()
        {
            Type tt = typeof(T);
            PropertyInfo[] ptmp = tt.PublicInstanceProperties();
            Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo p in ptmp)
                properties.Add(p.Name, p);
            List<T> queryResult = new List<T>();
            T dest;
            string name;
            object val;
            int i = 0;
            PropertyInfo pi;
            IDbValueConverter converter;
            Type pt;
            while (reader.Read())
            {
                dest = new T();
                for (i = 0; i < reader.FieldCount; ++i)
                {
                    name = reader.GetName(i);
                    val = reader.GetValue(i);
                    if (val == null || DBNull.Value.Equals(val))
                        continue;
                    if (!properties.TryGetValue(name, out pi))
                        continue;
                    pt = pi.PropertyType;
                    converter = db.GetValueConverter(pt);
                    if (converter != null)
                    {
                        pi.SetValue(dest, converter.Parse(val));
                        continue;
                    }
                    // 数据库引擎未配置值转换器时
                    pt = Nullable.GetUnderlyingType(pt) ?? pt; // 可空类型处理（防止值转换时隔儿屁）.
                    pi.SetValue(dest, Convert.ChangeType(val, pt));
                }
                queryResult.Add(dest);
            }
            return queryResult;
        }

        /// <summary>
        /// 将数据读取并构建为动态实体集合.
        /// </summary>
        /// <param name="reader">数据读取器.</param>
        /// <param name="converter">用于转换字段的值.</param>
        /// <returns></returns>
        internal static List<dynamic> ToDynamicList(this IDataReader reader, Func<string, object, Type, object> converter = null)
        {
            List<dynamic> queryResult = new List<dynamic>();
            int i = 0;
            DynamicEntity entity;
            string name;
            object val;
            Type t;
            while (reader.Read())
            {
                entity = new DynamicEntity();
                for (i = 0; i < reader.FieldCount; ++i)
                {
                    name = reader.GetName(i);
                    t = reader.GetFieldType(i);
                    if (converter == null)
                        val = reader.GetValue(i);
                    else
                        val = converter(name, reader.GetValue(i), t);
                    entity.SetPropertyValue(name, val, t);
                }
                queryResult.Add(entity);
            }
            return queryResult;
        }
    }
}
