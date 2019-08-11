using System;
using System.Collections.Generic;
using System.Text;
using Wunion.DataAdapter.EntityUtils;

namespace Wunion.IAC.Data.Entities
{
    
    
    /// <summary>
    /// 用于存储用户帐户基本信息的表
    /// </summary>
    [Serializable()]
    public class UserAccount : DataEntity
    {
        
        /// <summary>
        /// 创建一个 <see cref="Wunion.IAC.Data.Entities.UserAccount"/> 的对象实例.
        /// </summary>
        public UserAccount()
        {
        }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        [EntityProperty(AllowNull=false, PrimaryKey=true, IsIdentity=true, DefaultValue=0)]
        public int UID
        {
            get
            {
                return GetValue<int>("UID");
            }
            set
            {
                SetValue("UID", value);
            }
        }
        
        /// <summary>
        /// 所属分组（多个隶属分组用分号隔开）.
        /// </summary>
        [EntityProperty(AllowNull=true, DefaultValue=null)]
        public string Groups
        {
            get
            {
                return GetValue<string>("Groups");
            }
            set
            {
                SetValue("Groups", value);
            }
        }
        
        /// <summary>
        /// 用户名.
        /// </summary>
        [EntityProperty(AllowNull=false, DefaultValue=null)]
        public string Name
        {
            get
            {
                return GetValue<string>("Name");
            }
            set
            {
                SetValue("Name", value);
            }
        }
        
        /// <summary>
        /// 登入密码
        /// </summary>
        [EntityProperty(AllowNull=true, DefaultValue=null)]
        public string Password
        {
            get
            {
                return GetValue<string>("Password");
            }
            set
            {
                SetValue("Password", value);
            }
        }
        
        /// <summary>
        /// 身份编码（即借阅证号）.
        /// </summary>
        [EntityProperty(AllowNull=true, DefaultValue=null)]
        public string Code
        {
            get
            {
                return GetValue<string>("Code");
            }
            set
            {
                SetValue("Code", value);
            }
        }
        
        /// <summary>
        /// 手机号码.
        /// </summary>
        [EntityProperty(AllowNull=true, DefaultValue=null)]
        public string PhoneNo
        {
            get
            {
                return GetValue<string>("PhoneNo");
            }
            set
            {
                SetValue("PhoneNo", value);
            }
        }
        
        /// <summary>
        /// 状态（0 表示正常；1 表示待审核；2 表示已启用；）
        /// </summary>
        [EntityProperty(AllowNull=false, DefaultValue=0)]
        public int Status
        {
            get
            {
                return GetValue<int>("Status");
            }
            set
            {
                SetValue("Status", value);
            }
        }
        
        /// <summary>
        /// 权限.
        /// </summary>
        [EntityProperty(AllowNull=true, DefaultValue=null)]
        public string Permissions
        {
            get
            {
                return GetValue<string>("Permissions");
            }
            set
            {
                SetValue("Permissions", value);
            }
        }
        
        /// <summary>
        /// 创建日期
        /// </summary>
        [EntityProperty(AllowNull=true)]
        public System.DateTime CreateTime
        {
            get
            {
                return GetValue<System.DateTime>("CreateTime");
            }
            set
            {
                SetValue("CreateTime", value);
            }
        }
    }
}
