using Investment1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Investment1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Constructor to inject the ILogger<HomeController> dependency
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /Home/Index
        // Action method to handle requests to the Index page
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Privacy
        // Action method to handle requests to the Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: /Home/Error
        // Action method to handle error pages
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Create an ErrorViewModel with the current request ID
            // If Activity.Current is null, fallback to HttpContext.TraceIdentifier
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
