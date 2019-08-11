using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.EntityUtils;

namespace Wunion.IAC.Data.Entities
{
    
    
    /// <summary>
    /// 用于存储用户帐户基本信息的表（实体查询代理类）
    /// </summary>
    [EntityTable(TableName="UserAccount")]
    public class UserAccountAgent : EntityAgent
    {
        
        /// <summary>
        /// 创建一个 <see cref="Wunion.IAC.Data.Entities.UserAccountAgent"/> 的对象实例.
        /// </summary>
        public UserAccountAgent()
        {
        }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public FieldDescription UID
        {
            get
            {
                return GetField("UID");
            }
        }
        
        /// <summary>
        /// 所属分组（多个隶属分组用分号隔开）.
        /// </summary>
        public FieldDescription Groups
        {
            get
            {
                return GetField("Groups");
            }
        }
        
        /// <summary>
        /// 用户名.
        /// </summary>
        public FieldDescription Name
        {
            get
            {
                return GetField("Name");
            }
        }
        
        /// <summary>
        /// 登入密码
        /// </summary>
        public FieldDescription Password
        {
            get
            {
                return GetField("Password");
            }
        }
        
        /// <summary>
        /// 身份编码（即借阅证号）.
        /// </summary>
        public FieldDescription Code
        {
            get
            {
                return GetField("Code");
            }
        }
        
        /// <summary>
        /// 手机号码.
        /// </summary>
        public FieldDescription PhoneNo
        {
            get
            {
                return GetField("PhoneNo");
            }
        }
        
        /// <summary>
        /// 状态（0 表示正常；1 表示待审核；2 表示已启用；）
        /// </summary>
        public FieldDescription Status
        {
            get
            {
                return GetField("Status");
            }
        }
        
        /// <summary>
        /// 权限.
        /// </summary>
        public FieldDescription Permissions
        {
            get
            {
                return GetField("Permissions");
            }
        }
        
        /// <summary>
        /// 创建日期
        /// </summary>
        public FieldDescription CreateTime
        {
            get
            {
                return GetField("CreateTime");
            }
        }
        
        /// <summary>
        /// 创建该代理类代理的数据表上下文对象.
        /// </summary>
        /// <returns>创建该代理类代理的数据表上下文对象.</returns>
        public override TableMapper CreateContext()
        {
            return new UserAccountContext();
        }
    }
    
    /// <summary>
    /// 表示 <see cref="UserAccountAgent" /> 类代理的数据表上下文对象类型.
    /// </summary>
    public class UserAccountContext : TableContext<UserAccountAgent>
    {
        
        /// <summary>
        /// 创建一个 <see cref="Wunion.IAC.Data.Entities.UserAccountContext"/> 的对象实例.
        /// </summary>
        public UserAccountContext()
        {
        }
        
        /// <summary>
        /// 查询实体映射的表，并返回实体集合.
        /// <param name="selector">用于返回查询条件的筛选器.</param>
        /// </summary>
        /// <returns>返回查询查询结果实体集合..</returns>
        public List<UserAccount> Select(Func<UserAccountAgent, object> selector)
        {
            return base.Select<UserAccount>(selector);
        }
        
        /// <summary>
        /// 根据复杂的查询条件及分页等行为查询对应的表，并返回数据集合.
        /// <param name="action">用于指定查询条件、分页信息等.</param>
        /// </summary>
        /// <returns>返回查询查询结果实体集合..</returns>
        public List<UserAccount> Select(Action<UserAccountAgent, SelectBlock> action)
        {
            return base.Select<UserAccount>(action);
        }
        
        /// <summary>
        /// 向数据表中添加一条记录.
        /// </summary>
        /// <typeparam name="TEntity">数据表对应的实体类型.</typeparam>
        /// <param name="entity">实体对象.</param>
        /// <param name="trans">写事务的事务控制器.</param>
        /// <exception cref="Exception">当新增数据出错时引发该异常.</exception>
        public void Add(UserAccount entity, DBTransactionController trans = null)
        {
            base.Add<UserAccount>(entity, trans);
        }
        
        /// <summary>
        /// 更新数据表中的记录.
        /// </summary>
        /// <param name="entity">实体对象.</param>
        /// <param name="func">用于创建更新条件.</param>
        /// <exception cref="ArgumentNullException">当必备的参数为空时引发此异常.</exception>
        /// <exception cref="Exception">当执行更新时产生错误时引发此异常.</exception>
        /// <returns>返回受影响的记录数.</returns>
        public int Update(UserAccount entity, Func<UserAccountAgent, object[]> func)
        {
            return base.Update<UserAccount>(entity, func);
        }
        
        /// <summary>
        /// 在事务中更新数据表中的记录.
        /// </summary>
        /// <param name="trans">事务控制器.</param>
        /// <param name="entity">实体对象.</param>
        /// <param name="func">用于创建更新条件.</param>
        /// <exception cref="ArgumentNullException">当必备的参数为空时引发此异常.</exception>
        /// <exception cref="Exception">当执行更新时产生错误时引发此异常.</exception>
        /// <returns>返回受影响的记录数.</returns>
        public int Update(DBTransactionController trans, UserAccount entity, Func<UserAccountAgent, object[]> func)
        {
            return base.Update<UserAccount>(trans, entity, func);
        }
    }
}
