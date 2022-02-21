using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于映射实体对象与数据库表中的行之间的关系.
    /// </summary>
    public class DbConversionMapping
    {
        /// <summary>
        /// 实体对象的属性信息.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// 实体对象属性与数据库字段的映射信息.
        /// </summary>
        public TableFieldAttribute Attribute { get; private set; }

        /// <summary>
        /// 字段的自动编号设置.
        /// </summary>
        public DbColumnIdentity Identity { get; internal set; }

        /// <summary>
        /// 字段的外键约束设置.
        /// </summary>
        public DbForeignKey ForeignKey { get; internal set; }

        public int GenerateOrder { get; internal set; }

        /// <summary>
        /// 创建一个 <see cref="DbConversionMapping"/> 的对象实例.
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="attr"></param>
        internal DbConversionMapping(PropertyInfo pi, TableFieldAttribute attr)
        {
            Property = pi;
            Attribute = attr;
            Identity = null;
            ForeignKey = null;
            GenerateOrder = 0;
        }

        /// <summary>
        /// 获取指定类型的实体的数据库映射信息集合.
        /// </summary>
        /// <param name="entityType">实体的类型.</param>
        /// <returns></returns>
        public static List<DbConversionMapping> Get(Type entityType)
        {
            List<DbConversionMapping> list = new List<DbConversionMapping>();
            // 先获取所有实体类型的公共属性.
            PropertyInfo[] array;
            do
            {
                array = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (array != null && array.Length > 0)
                    break;
                entityType = entityType.BaseType;
            } while (entityType != null);
            if (array == null || array.Length < 1)
                return list;
            //然后获得每个属性映射信息
            TableFieldAttribute fieldAttr;
            IdentityAttribute identityAttr;
            ForeignKeyAttribute fkAttr;
            GenerateOrderAttribute order;
            DbConversionMapping mapping;
            foreach (PropertyInfo pi in array)
            {
                fieldAttr = pi.GetCustomAttribute<TableFieldAttribute>();
                if (fieldAttr == null)
                    continue;
                mapping = new DbConversionMapping(pi, fieldAttr);
                list.Add(mapping);
                identityAttr = pi.GetCustomAttribute<IdentityAttribute>();
                fkAttr = pi.GetCustomAttribute<ForeignKeyAttribute>();
                order = pi.GetCustomAttribute<GenerateOrderAttribute>();
                mapping.Identity = identityAttr?.CreateDbIdentity();
                mapping.ForeignKey = fkAttr?.CreateForeignKey();
                if (order != null)
                    mapping.GenerateOrder = order.Index;
            }
            return (from p in list orderby p.GenerateOrder ascending select p).ToList();
        }
    }
}
