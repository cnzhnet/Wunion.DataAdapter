using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.DbInterop;
using Wunion.DataAdapter.Kernel.Querying;
using Wunion.DataAdapter.Kernel.CommandBuilders;
using Wunion.DataAdapter.CodeFirstDemo.Data;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;
using Wunion.DataAdapter.CodeFirstDemo.Data.Models;
using GRPDAO = Wunion.DataAdapter.CodeFirstDemo.Data.Domain.UserAccountGroupDao;

namespace Wunion.DataAdapter.CodeFirstDemo.Services
{
    /// <summary>
    /// 应用程序的用户账户服务.
    /// </summary>
    public class UserAccountService : ApplicationService<UserAccount>
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
            creation = new DateTime(creation.Year, creation.Month, creation.Day, creation.Hour, creation.Minute, creation.Second); //去掉日期的毫秒部份.
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
        /// 若指定的字段值已被占用时触发异常.
        /// </summary>
        /// <param name="field">要检测的字段名称.</param>
        /// <param name="data">实体对象.</param>
        /// <param name="batch"></param>
        /// <exception cref="Exception"></exception>
        private void ThrowIfFieldOccupied(string field, UserAccount data, BatchCommander batch)
        {
            int existing = 0;
            if (field == nameof(data.Name))
            {
                if (string.IsNullOrEmpty(data.Name))
                    throw new Exception($"${nameof(data.Name)} 不能为空.");
                existing = QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                    .Where<UserAccountDao>(p => new object[] { p.Name == data.Name })
                    .Select(p => new IDescription[] { Fun.Count(1) })
                    .Count(batch);
                if (existing > 0)
                    throw new Exception($"{nameof(data.Name)} 已被占用.");
                return;
            }
            if (field == nameof(data.PhoneNumber) && !string.IsNullOrEmpty(data.PhoneNumber))
            {
                existing = QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                    .Where<UserAccountDao>(p => new object[] { p.PhoneNumber == data.PhoneNumber })
                    .Select(p => new IDescription[] { Fun.Count(1) })
                    .Count(batch);
                if (existing > 0)
                    throw new Exception($"{nameof(data.PhoneNumber)} 已被占用.");
                return;
            }
            if (field == nameof(data.Email) && !string.IsNullOrEmpty(data.Email))
            {
                existing = QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                    .Where<UserAccountDao>(p => new object[] { p.Email == data.Email })
                    .Select(p => new IDescription[] { Fun.Count(1) })
                    .Count(batch);
                if (existing > 0)
                    throw new Exception($"{nameof(data.Email)} 已被占用.");
                return;
            }
        }

        /// <summary>
        /// 添加用户账户.
        /// </summary>
        /// <param name="data">用户账户数据.</param>
        public override void Add(UserAccount data)
        {
            using (BatchCommander batch = new BatchCommander(db.DbEngine))
            {
                ThrowIfFieldOccupied(nameof(data.Name), data, batch);
                ThrowIfFieldOccupied(nameof(data.PhoneNumber), data, batch);
                ThrowIfFieldOccupied(nameof(data.Email), data, batch);
                data.Password = ProtectPassword(data.Password, data.Creation.Value);
                db.UserAccounts.Add(data);
            }
        }

        /// <summary>
        /// 删除用户账户.
        /// </summary>
        /// <param name="condition">删除条件.</param>
        public override void Delete(object condition)
        {
            int uid = Convert.ToInt32(condition);
            Delete<UserAccount, UserAccountDao>(db.UserAccounts, p => new object[] { 
                p.UID == uid
            });
        }

        /// <summary>
        /// 修改用户账户.
        /// </summary>
        /// <param name="data">更新的用户数据.</param>
        public override void Update(UserAccount data)
        {
            using (BatchCommander batch = new BatchCommander(db.DbEngine)) 
            {
                UserAccount ua = QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                    .Where<UserAccountDao>(p => new object[] { p.UID == data.UID })
                    .Select(p => p.First.All)
                    .Build()
                    .ToEntityList<UserAccount>(batch)
                    .FirstOrDefault();
                if (ua == null)
                    throw new Exception("指定的用户账户已不存在.");
                if (data.PhoneNumber != ua.PhoneNumber)
                    ThrowIfFieldOccupied(nameof(data.PhoneNumber), data, batch);
                if (data.Email != ua.Email)
                    ThrowIfFieldOccupied(nameof(data.Email), data, batch);
                if (!string.IsNullOrEmpty(data.Password))
                    data.Password = ProtectPassword(data.Password, ua.Creation.Value);
                else
                    data.Password = ua.Password;
                db.UserAccounts.Update(data, batch);
            }
        }

        /// <summary>
        /// 获取所有用户账户列表.
        /// </summary>
        /// <returns></returns>
        public List<UserDataModel> List()
        {
            return QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                .Select(p => p.First.All)
                .Build((p, options) => { options.OrderBy(p.First.UID, OrderByMode.ASC); })
                .ToList<UserDataModel>();
        }

        /// <summary>
        /// 用户账户登录.
        /// </summary>
        /// <param name="name">用户名或邮箱号.</param>
        /// <param name="password">登录密码.</param>
        /// <returns></returns>
        public UserAuthorization LogIn(string name, string password)
        {
            UserAccount account;
            List<UserAccountGroup> groups;
            using (BatchCommander batch = new BatchCommander(db.DbEngine))
            {
                account = QueryBuilder<UserAccountDao>.Create(db.UserAccounts)
                    .Where<UserAccountDao>(p => new object[] { p.Name == name | p.Email == name })
                    .Select(p => p.First.All)
                    .Build()
                    .ToEntityList<UserAccount>()
                    .FirstOrDefault();
                if (account == null)
                    throw new Exception("指定的用户账户不存在.");
                if (account.Password != ProtectPassword(password, account.Creation.Value))
                    throw new Exception("无效的登录密码.");
                if (account.Status != UserAccountStatus.Enabled)
                    throw new Exception("此用户账户已被锁定或禁用.");
                // 获取权限.
                groups = QueryBuilder<GRPDAO>.Create(db.UserAccountGroups)
                    .Where<GRPDAO>(p => new object[] { p.Id.In(account.Groups.Select(p => (object)p).ToArray()) })
                    .Select(p => p.First.All)
                    .Build()
                    .ToEntityList<UserAccountGroup>();
            }
            List<int> permissions = new List<int>();
            foreach (UserAccountGroup grp in groups)
            {
                if (grp.Permissions == null)
                    continue;
                if (grp.Permissions.Count < 1)
                    continue;
                permissions.AddRange(grp.Permissions);
            }
            return new UserAuthorization { 
                UID = account.UID, 
                Permissions = permissions.Distinct().ToList() 
            };
        }
    }
}
