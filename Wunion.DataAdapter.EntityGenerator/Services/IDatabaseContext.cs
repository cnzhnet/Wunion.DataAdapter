using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.EntityUtils.CodeProvider;

namespace Wunion.DataAdapter.EntityGenerator.Services
{
    /// <summary>
    /// 用于获取数据库信息的接口.
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// 获取或设置数据引擎.
        /// </summary>
        DataEngine DbEngine { get; }

        /// <summary>
        /// 获取数据库表的信息.
        /// </summary>
        /// <returns></returns>
        List<TableInfoModel> GetTables();
    }
}
