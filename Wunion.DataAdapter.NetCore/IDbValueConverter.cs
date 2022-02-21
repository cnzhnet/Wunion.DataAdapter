using System;

namespace Wunion.DataAdapter.Kernel
{
    /// <summary>
    /// 用于实现从指定的自定义类型将值转换到数据库支持的值的转换器接口.
    /// </summary>
    public interface IDbValueConverter
    {
        /// <summary>
        /// 将指定的值转换到自定义类型.
        /// </summary>
        /// <param name="value">要转换的值.</param>
        /// <returns>返回成功转换后的类型值.</returns>
        object Parse(object value);

        /// <summary>
        /// 将自定义类型的值转换为给定的数据库支持的值类型.
        /// </summary>
        /// <param name="value">原始类型值.</param>
        /// <param name="dest">要转换的目标对象.</param>
        /// <returns></returns>
        object ConvertTo(object value, Type dest);
    }

    /// <summary>
    /// 用于实现从指定的自定义类型将值转换到数据库支持的值的转换器基类
    /// （可在构造函数中设置 DefaultValue 属性来自定义默认的值）.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public abstract class DbValueConverter<TSource> : IDbValueConverter
    {
        /// <summary>
        /// 获取或设置默认值.
        /// </summary>
        protected TSource DefaultValue { get; set; }

        /// <summary>
        /// 创建一个 <see cref="DbValueConverter{TSource}"/> 的对象实例.
        /// </summary>
        protected DbValueConverter()
        {
            DefaultValue = default(TSource);
        }

        /// <summary>
        /// 实现从原始值到数据库支持的目标值的转换.
        /// </summary>
        /// <param name="value">原始值.</param>
        /// <param name="dest">数据库支持的目标值的类型.</param>
        /// <param name="buffer">用于输出转换结果的缓冲区.</param>
        protected abstract void ConvertTo(TSource value, Type dest, out object buffer);

        /// <summary>
        /// 将自定义类型的值转换为给定的数据库支持的值类型.
        /// </summary>
        /// <param name="value">原始类型值.</param>
        /// <param name="dest">要转换的目标对象.</param>
        /// <returns></returns>
        public object ConvertTo(object value, Type dest)
        {
            object buffer = null;
            ConvertTo((TSource)value, dest, out buffer);
            return buffer;
        }

        /// <summary>
        /// 实现从数据库值转换为自定义类型的值.
        /// </summary>
        /// <param name="value">数据库中查获的值.</param>
        /// <param name="buffer">用于输出转换结果的缓冲区.</param>
        protected abstract void Parse(object value, ref TSource buffer);

        /// <summary>
        /// 将指定的值转换到自定义类型.
        /// </summary>
        /// <param name="value">要转换的值.</param>
        /// <returns>返回成功转换后的类型值.</returns>
        public object Parse(object value)
        {
            TSource buffer = DefaultValue;
            Parse(value, ref buffer);
            return buffer;
        }
    }
}
