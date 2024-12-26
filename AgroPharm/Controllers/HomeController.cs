using System.Diagnostics;
using AgroPharm.Interfaces;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AgroPharm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MarketRepository _market;

        public HomeController(ILogger<HomeController> logger, MarketRepository market)
        {
            _logger = logger;
            _market = market;   
        }

        public async  Task<IActionResult> Index()
        {
            try
            {
                var res = await _market.GetMarketList();
                return View(res);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction("ServerError", "Product");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Product");
            }
            
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult PrintReport()
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
