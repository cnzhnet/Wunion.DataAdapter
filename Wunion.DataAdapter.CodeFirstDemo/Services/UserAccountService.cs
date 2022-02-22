using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.Querying;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;

using UPDAO = Wunion.DataAdapter.CodeFirstDemo.Data.Domain.UserAccountPermissionDao;

namespace Wunion.DataAdapter.CodeFirstDemo.Services
{
    /// <summary>
    /// 应用程序的用户账户服务.
    /// </summary>
    public class UserAccountService : ApplicationService<UserDataModel>
    {
        /// <summary>
        /// 创建一个应用程序的用户账户服务.
        /// </summary>
        /// <param name="context">数据库上下文对象.</param>
        public UserAccountService(MyDbContext context) : base(context)
        { }

        /// <summary>
        /// 保护输入的密码，并返回受保护后的密码.
        /// </summary>
        /// <param name="input">输入的密码.</param>
        /// <param name="creation">用户账户的创建日期.</param>
        /// <returns></returns>
        public string ProtectPassword(string input, DateTime creation)
        {
            byte[] buffer = BitConverter.GetBytes(creation.Ticks);
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                stream.Write(buffer, 0, 4);
                stream.Write(Encoding.ASCII.GetBytes(input));
                stream.Write(buffer, 4, 4);
                buffer = stream.ToArray();
            }
            using (SHA256 sha = SHA256.Create())
                buffer = sha.ComputeHash(buffer);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 从数据模型中获取用户及权限实体.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="account"></param>
        /// <param name="permission"></param>
        private void GetEntities(UserDataModel model, out UserAccount account, out UserAccountPermission permission)
        {
            account = new UserAccount {
                UID = model.UID,
                Name = model.Name,
                Password = ProtectPassword(model.Password, model.Creation),
                User = model.User,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Status = model.Status,
                Creation = model.Creation
            };

            permission = new UserAccountPermission {
                UID = model.UID,
                Permissions = model.Permissions
            };
        }

        /// <summary>
        /// 添加用户账户.
        /// </summary>
        /// <param name="data">用户账户数据.</param>
        public override void Add(UserDataModel data)
        {
            UserAccount ua;
            UserAccountPermission permission;
            GetEntities(data, out ua, out permission);
            // 将用户账户信息写入数据库.
            using (DBTransactionController trans = db.DbEngine.BeginTrans())
            {
                db.UserAccounts.Add(ua, trans);
                ua.UID = Convert.ToInt32(trans.DBA.SCOPE_IDENTITY);
                permission.UID = ua.UID;
                db.UserPermissions.Add(permission, trans);
                trans.Commit();
            }
            data.UID = ua.UID;
        }

        /// <summary>
        /// 删除用户账户.
        /// </summary>
        /// <param name="condition">删除条件.</param>
        public override void Delete(object condition)
        {
            int uid = Convert.ToInt32(condition);
            using (DBTransactionController trans = db.DbEngine.BeginTrans())
            {
                db.UserPermissions.Delete<UPDAO>(p => new object[] { 
                    p.UID == uid
                }, trans);

                db.UserAccounts.Delete<UserAccountDao>(p => new object[] { p.UID == uid }, trans);
                trans.Commit();
            }
        }

        /// <summary>
        /// 修改用户账户.
        /// </summary>
        /// <param name="data">更新的用户数据.</param>
        public override void Update(UserDataModel data)
        {
            UserAccount ua;
            UserAccountPermission permission;
            GetEntities(data, out ua, out permission);

            using (DBTransactionController trans = db.DbEngine.BeginTrans())
            {
                db.UserAccounts.Update(ua, trans);
                db.UserPermissions.Update(permission, trans);
                trans.Commit();
            }
        }

        /// <summary>
        /// 获取所有用户账户列表.
        /// </summary>
        /// <returns></returns>
        public List<UserDataModel> List()
        {
            return QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                .Include<UPDAO>(p => new object[] { p.First.UID == p.tbl<UPDAO>().UID })
                .Select(p => p.First.All.Concat(new IDescription[] { p.tbl<UPDAO>().Permissions }).ToArray())
                .Build((p, options) => { options.OrderBy(p.First.UID, OrderByMode.ASC); })
                .ToList<UserDataModel>();
        }

        /// <summary>
        /// 获到指定 UID 的用户账户.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public UserDataModel GetById(int uid)
        {
            return QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                .Include<UPDAO>(p => new object[] { p.First.UID == p.tbl<UPDAO>().UID })
                .Where<UserAccountDao>(p => new object[] { p.UID == uid })
                .Select(p => p.First.All.Concat(new IDescription[] { p.tbl<UPDAO>().Permissions }).ToArray())
                .Build((p, options) => { options.OrderBy(p.First.UID, OrderByMode.ASC); })
                .ToList<UserDataModel>()
                .FirstOrDefault();
        }

        /// <summary>
        /// 用户账户登录.
        /// </summary>
        /// <param name="name">用户名或邮箱号.</param>
        /// <param name="password">登录密码.</param>
        /// <returns></returns>
        public UserAuthorization LogIn(string name, string password)
        {
            UserDataModel model = QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                .Include<UPDAO>(p => new object[] { p.First.UID == p.tbl<UPDAO>().UID })
                .Where<UserAccountDao>(p => new object[] { 
                    p.Name == name | p.Email == name
                })
                .Select(p => p.First.All.Concat(new IDescription[] { p.tbl<UPDAO>().Permissions }).ToArray())
                .Build()
                .ToList<UserDataModel>()
                .FirstOrDefault();
            if (model == null)
                throw new WebApiException(404, "指定的用户不存在.");
            password = ProtectPassword(password, model.Creation);
            if (model.Password != password)
                throw new WebApiException(500, "无效的登录密码.");
            return new UserAuthorization { UID = model.UID, Permissions = model.Permissions };
        }
    }
}
