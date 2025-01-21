using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AgroPharm.Controllers
{
    public class CurrencyController : Controller
    {
        CurrencyRepo _currencyRepo;
        public CurrencyController(CurrencyRepo currencyRepo)
        {
            _currencyRepo = currencyRepo;
        }
        public IActionResult Index()
        {
            try
            {
                IEnumerable<Currency>? getCurrency = _currencyRepo.GetCurrencyList();
                var getCurrencyNow = Convert.ToDecimal(_currencyRepo.GetLastCurrency());
                var model = new ViewModel()
                {
                    Currency = getCurrency,
                    CurrencyNow = getCurrencyNow
                };
                return View(model);
            }
            catch(MySqlException mex)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Currency currency)
        {
            try
            {
                if (currency.USDtoTJS ==0)
                {
                    return Json(new { success = false, message = "Курс валют не указано" });
                }
                _currencyRepo.AddCurrency(currency);
                return Json(new { success = true, message = "Курс валют успешно обновлено!" });
               
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }
    }
}
