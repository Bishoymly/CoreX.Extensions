using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreX.Extensions.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace CoreX.Dashboard.Controllers
{
    public class DeveloperDashboardController : Controller
    {
        public IMetricsService Metrics { get; }

        public DeveloperDashboardController(IMetricsService metrics)
        {
            Metrics = metrics;
        }        

        public IActionResult Index()
        {
            return View(Metrics);
        }
    }
}
