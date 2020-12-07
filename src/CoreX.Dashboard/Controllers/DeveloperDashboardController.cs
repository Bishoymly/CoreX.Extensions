using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace CoreX.Dashboard.Controllers
{
    public class DeveloperDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
