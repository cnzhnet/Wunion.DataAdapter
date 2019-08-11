using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Wunion.IAC.Data.Entities;
using Wunion.DataAdapter.EntityUtils;
using Wunion.DataAdapter.Kernel.SQLServer;
using Wunion.DataAdapter.Kernel.SQLServer.CommandParser;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.NetCore.Demo.Controllers
{
    public class EntityUtilsTestController : BaseController
    {
        private static DatabaseContext db;
        public EntityUtilsTestController(IHostingEnvironment hosting) : base(hosting)
        {
            if (db == null)
            {
                db = DatabaseContext.Initialize(() =>
                {
                    DatabaseContext.ContextOptions options = new DatabaseContext.ContextOptions();
                    options.ReadAccess = new SqlServerDbAccess();
                    options.ReadAccess.ConnectionString = "Server=;Database=;User ID=;Password=;";
                    options.WriteAccess = options.ReadAccess;
                    options.Parser = new SqlServerParserAdapter();
                    return options;
                });
            }
        }

        [Route("EntityUtilsTest/Users/{gid:int}")]
        public IActionResult QueryUsers(int gid)
        {
            List<UserAccount> usersByGroup = db.Table<UserAccountContext>().Select(p => p.Groups.Like(string.Format("{0};", gid), LikeMatch.Center));
            return Json(usersByGroup);
        }
    }
}
