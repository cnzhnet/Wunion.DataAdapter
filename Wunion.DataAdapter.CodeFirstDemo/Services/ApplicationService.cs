using System;
using System.Collections.Generic;
using System.Linq;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;

namespace Wunion.DataAdapter.CodeFirstDemo.Services
{
    /// <summary>
    /// 应用程序服务的基础实现类型.
    /// </summary>
    /// <typeparam name="TData">数据实体或模型的类型名称.</typeparam>
    public abstract class ApplicationService<TData> where TData : class, new()
    {
        /// <summary>
        /// 创建一个 <see cref="ApplicationService{TData}"/> 的对象实例.
        /// </summary>
        /// <param name="context">数据库上下文对象.</param>
        protected ApplicationService(MyDbContext context)
        {
            db = context;
        }

        /// <summary>
        /// 数据库上下文对象.
        /// </summary>
        protected MyDbContext db { get; private set; }

        /// <summary>
        /// 添加数据.
        /// </summary>
        /// <param name="data">要添加的数据.</param>
        public abstract void Add(TData data);

        /// <summary>
        /// 更新数据.
        /// </summary>
        /// <param name="data">要更新的数据.</param>
        public abstract void Update(TData data);

        /// <summary>
        /// 删除数据.
        /// </summary>
        /// <param name="condition">删除条件.</param>
        public abstract void Delete(object condition);
    }
}
