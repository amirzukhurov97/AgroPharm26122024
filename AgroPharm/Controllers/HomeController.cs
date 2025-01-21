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
        private readonly BuyProductRepo _buyProductRepo;
        private readonly SellProductRepo _sellProductRepo;
        private readonly ReturnInRepo _returnInRepo;
        private readonly ReturnOutRepo _returnOutRepo;

        public HomeController(ILogger<HomeController> logger, MarketRepository market, BuyProductRepo buyProductRepo, SellProductRepo sellProductRepo, ReturnInRepo returnInRepo, ReturnOutRepo returnOutRepo)
        {
            _logger = logger;
            _market = market;
            _buyProductRepo = buyProductRepo;
            _sellProductRepo = sellProductRepo;
            _returnInRepo = returnInRepo;
            _returnOutRepo = returnOutRepo;
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
        public IActionResult PrintReport()
        {
            return View();
        }

        [Route("/Shared/ServerError")]
        public IActionResult ServerError()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            var buyProductTotal = _buyProductRepo.GetBuyProductTotal();
            var sellProductTotal = _sellProductRepo.GetSellProductTotal();
            var returnInTotal = _returnInRepo.GetReturnInTotal();
            var returnOutTotal = _returnOutRepo.GetReturnOutTotal();

            var model = new ViewModel()
            {
                BuyProductResponse = buyProductTotal,
                SellResponse = sellProductTotal,
                ReturnInResponse = returnInTotal,
                ReturnOutResponse = returnOutTotal
            };

            return View(model);
        }

    }
}
