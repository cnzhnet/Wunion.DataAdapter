using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Wunion.DataAdapter.NetCore.Demo.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IHostingEnvironment hosting) : base(hosting)
        { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {            
            return View();
        }

        public IActionResult Contact()
        {            
            return View();
        }
    }
}