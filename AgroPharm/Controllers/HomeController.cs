using System.Diagnostics;
using AgroPharm.Interfaces;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

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

        //public async  Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var res = _market.GetMarketList();                
        //        return View(res);              
        //    }
        //    catch(MySqlException me)
        //    {
        //        return Json(new { success = false, message = $"Произошла ошибка при работе с БД: {me.Message}" });
        //    }
        //    catch (Exception ex)
        //    {
        //        //return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
        //        return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
        //    }
            
        //}
        public async Task<IActionResult> Index()
        {
            try
            {
                var markets = _market.GetMarketList();
                return View(markets);
                 
            }
            catch (MySqlException me)
            {
                return RedirectToAction("ServerError");
                //return Json(new { success = false, message = $"Ошибка при работе с БД: {me.Message}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
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

        [Route("/Shared/ServerError")]
        public IActionResult ServerError()
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
