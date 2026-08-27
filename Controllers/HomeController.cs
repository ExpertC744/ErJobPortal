using System.Diagnostics;
using ErJobPortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace ErJobPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Organization()
        {
            return View();
        }
        public IActionResult TeamInvolved()
        {
            return View();
        }
        public IActionResult Terms_Cond()
        {
            return View();
        }
        public IActionResult Org_blog()
        {
            return View();
        }
        public IActionResult Benefits()
        {
            return View();
        }
        public IActionResult Tips()
        {
            return View();
        }
        public IActionResult TopCarriers()
        {
            return View();
        }
        public IActionResult Transformation()
        {
            return View();
        }
        public IActionResult Embracing_change()
        {
            return View();
        }
        public IActionResult Contact()
        {
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
