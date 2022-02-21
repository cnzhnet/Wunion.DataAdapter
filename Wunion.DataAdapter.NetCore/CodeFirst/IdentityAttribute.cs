using System;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.Kernel.CodeFirst
{
    /// <summary>
    /// 用于标记自增序列字段.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IdentityAttribute : Attribute
    {
        /// <summary>
        /// 自增长的初始值.
        /// </summary>
        public int InitValue { get; set; }

        /// <summary>
        /// 自动增长的增量值.
        /// </summary>
        public int Increment { get; set; }

        /// <summary>
        /// 创建一个 <see cref="IdentityAttribute"/> 的对象实例.
        /// </summary>
        public IdentityAttribute()
        {
            InitValue = 0;
            Increment = 1;
        }

        /// <summary>
        /// 创建一个 <see cref="IdentityAttribute"/> 的对象实例.
        /// </summary>
        /// <param name="init">初始值.</param>
        /// <param name="inc">递增步长.</param>
        public IdentityAttribute(int init, int inc)
        {
            InitValue = init;
            Increment = inc;
        }

        /// <summary>
        /// 创建数据库的自增长信息.
        /// </summary>
        /// <returns></returns>
        public DbColumnIdentity CreateDbIdentity()
        {
            if (Increment < 1)
                return null;
            return new DbColumnIdentity(InitValue, Increment);
        }
    }
}
