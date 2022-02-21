using System;
using System.Collections.Generic;
using System.Text;

namespace Wunion.DataAdapter.Kernel
{
    // <summary>
    /// 表示数据库值的类型转换器选项设置信息对象类型.
    /// </summary>
    public class DbValueConverterOptions
    {
        /// <summary>
        /// 用于存储数据库值的转换器支持.
        /// </summary>
        private Dictionary<Type, IDbValueConverter> Converters;

        /// <summary>
        /// 创建一个 <see cref="DbValueConverterOptions"/> 的对象实例.
        /// </summary>
        internal DbValueConverterOptions()
        {
            Converters = new Dictionary<Type, IDbValueConverter>();
        }

        /// <summary>
        /// 添加一个数据库类型转换器.
        /// </summary>
        /// <param name="dest">添加的转换器针对的类型.</param>
        /// <param name="converter">转换器对象实例.</param>
        public void Add(Type dest, IDbValueConverter converter)
        {
            if (Converters.ContainsKey(dest))
                Converters[dest] = converter;
            else
                Converters.Add(dest, converter);
        }

        /// <summary>
        /// 获取指定类型的数据库值转换器（若不存在则返回 null）.
        /// </summary>
        /// <param name="valueType">值的类型.</param>
        /// <returns></returns>
        public IDbValueConverter Get(Type valueType)
        {
            IDbValueConverter converter = null;
            if (!Converters.TryGetValue(valueType, out converter))
                return null;
            return converter;
        }
    }
}
