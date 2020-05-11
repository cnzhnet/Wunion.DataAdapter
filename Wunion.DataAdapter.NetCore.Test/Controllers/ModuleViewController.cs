using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Wunion.DataAdapter.NetCore.Test.Models;

namespace Wunion.DataAdapter.NetCore.Test.Controllers
{
    /// <summary>
    /// 用于提模块视图内容的 webapi .
    /// </summary>
    public class ModuleViewController : Controller
    {
        /// <summary>
        /// 创建一个 <see cref="ModuleViewController"/> 的对象实例.
        /// </summary>
        public ModuleViewController()
        { }

        /// <summary>
        ///  返回错误页面的视图.
        /// </summary>
        /// <param name="title">错误信息的标题.</param>
        /// <param name="message">错误信息的详细信息.</param>
        /// <returns></returns>
        private IActionResult ErrorView(string title, string message)
        {
            return View("~/Pages/Shared/_Error.cshtml", new ErrorViewModel {
                Context = HttpContext,
                Title = "产生了访问错误",
                Message = message
            });
        }

        /// <summary>
        /// 用于获取指定模块视图的 webapi .
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("/moduleView/Get"), HttpPost]
        public IActionResult GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return ErrorView("视图请求的错误", "缺少必要的视图名称：name 参数.");
            try
            {
                StringBuilder viewPath = new StringBuilder("~/Pages/");
                string[] array = name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length - 1; ++i)
                    viewPath.AppendFormat("{0}/", array[i]);
                viewPath.AppendFormat("_{0}.cshtml", array.Last());
                ModuleViewModel model;
                switch (name.ToLower())
                {
                    case "shared/dataeditor":
                        model = new DataEditorViewModel { Context = HttpContext, Name = name };
                        break;
                    default:
                        model = new ModuleViewModel { Context = HttpContext, Name = name };
                        break;
                }
                return View(viewPath.ToString(), model);
            }
            catch (Exception Ex)
            {
                return ErrorView("产生了访问错误", Ex.Message);
            }
        }
    }
}
