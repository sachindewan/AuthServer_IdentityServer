using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcWebClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MvcWebClient.Services;

namespace MvcWebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJobservice jobservice;
        public HomeController(ILogger<HomeController> logger, IJobservice _jobservice)
        {
            _logger = logger;
            jobservice = _jobservice;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var jobs = await jobservice.GetJob();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
