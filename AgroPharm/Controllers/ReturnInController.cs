using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace AgroPharm.Controllers
{
    public class ReturnInController : Controller
    {
        private readonly ReturnInRepo _returnIn;
        private readonly ProductRepo _product;
        private readonly CustomerRepo _customer;
        private readonly MarketRepository _marketRepository;

        public ReturnInController(ReturnInRepo returnIn, ProductRepo product, CustomerRepo customer, MarketRepository marketRepository)
        {
            _returnIn = returnIn;
            _product = product;
            _customer = customer;
            _marketRepository = marketRepository;
        }
        public IActionResult Index()
        {
            try
            {
                return View(_returnIn.GetReturnInProducts());
            }
            catch (MySqlException me)
            {
                return RedirectToAction("/Home/ServerError");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: BuyController/Create
        public ActionResult Create()
        {
            var products = _product.GetProducts().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.ProductName
            }).ToList();

            var customers = _customer.GetCustomers().Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.customerName
            }).ToList();

            var model = new ViewModel
            {
                Products = products,
                Customers = customers,
                ReturnInRequest = new ReturnInRequest()
            };
            return View(model);
        }

        // POST: BuyController/Create
        [HttpPost]
        public ActionResult Create(ViewModel returnIn)
        {
            try
            {
                if (returnIn.ReturnInRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (returnIn.ReturnInRequest.CustomerNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (returnIn.ReturnInRequest.ReturnInProductPrice == 0 || returnIn.ReturnInRequest.ReturnInProductPriceUSD == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (returnIn.ReturnInRequest.ReturnInProductObem == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                // Сохранение данных в базу
                var returnInproducts = new ReturnInRequest
                {
                    ProductNameID = returnIn.ReturnInRequest.ProductNameID,
                    CustomerNameID = returnIn.ReturnInRequest.CustomerNameID,
                    ReturnInProductPrice = returnIn.ReturnInRequest.ReturnInProductPrice,
                    ReturnInProductPriceUSD = returnIn.ReturnInRequest.ReturnInProductPriceUSD,
                    ReturnInProductObem = returnIn.ReturnInRequest.ReturnInProductObem,
                    ReturnInProductSumPrice = returnIn.ReturnInRequest.ReturnInProductSumPrice,
                    ReturnInProductSumPriceUSD = returnIn.ReturnInRequest.ReturnInProductSumPriceUSD,
                    ReturnInProductDate = returnIn.ReturnInRequest.ReturnInProductDate,
                    ReturnInComment = returnIn.ReturnInRequest.ReturnInComment
                };
                var market = new MarketRequest
                {
                    ProductNameID = returnIn.ReturnInRequest.ProductNameID,
                    obemProducts = returnIn.ReturnInRequest.ReturnInProductObem
                };
                _returnIn.Create(returnInproducts);
                _marketRepository.IncomeProduct(market);
                return Json(new { success = true, message = "Товар успешно возврашён в склад!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

        // GET: BuyController/Edit/5
        public ActionResult Edit(int id)
        {
            var products = _product.GetProducts().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.ProductName
            }).ToList();

            var customers = _customer.GetCustomers().Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.customerName
            }).ToList();

            var itemsRecord = _returnIn.GetReturnInProducts().FirstOrDefault(b => b.Id == id);
            ViewModel? model = null;

            if (itemsRecord != null)
            {
                model = new ViewModel
                {
                    Products = products,
                    Customers = customers,
                    ReturnInRequest = new ReturnInRequest()
                    {
                        Id = id,
                        ProductNameID = itemsRecord?.ProductNameID ?? 0,
                        CustomerNameID = itemsRecord?.CustomerNameID ?? 0,
                        ReturnInProductPrice = itemsRecord?.ReturnInProductPrice ?? 0,
                        ReturnInProductPriceUSD = itemsRecord?.ReturnInProductPriceUSD ?? 0,
                        ReturnInProductObem = itemsRecord?.ReturnInProductObem ?? 0,
                        ReturnInProductSumPrice = itemsRecord?.ReturnInSumProductPrice ?? 0,
                        ReturnInProductSumPriceUSD = itemsRecord?.ReturnInSumProductPriceUSD ?? 0,
                        ReturnInComment = itemsRecord?.ReturnInComment
                    },
                    ReturnInResponse = itemsRecord,
                    tempQuantity = itemsRecord.ReturnInProductObem
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ViewModel returnIn)
        {
            try
            {
                if (returnIn.ReturnInRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (returnIn.ReturnInRequest.CustomerNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (returnIn.ReturnInRequest.ReturnInProductPriceUSD == 0 || returnIn.ReturnInRequest.ReturnInProductPrice == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (returnIn.ReturnInRequest.ReturnInProductObem == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                var producrMarket = _marketRepository.GetMarketList().FirstOrDefault(p => p.ProductNameID == returnIn.ReturnInRequest.ProductNameID);
                if (returnIn.tempQuantity > returnIn.ReturnInRequest.ReturnInProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = returnIn.ReturnInRequest.ProductNameID,
                        obemProducts = returnIn.tempQuantity - returnIn.ReturnInRequest.ReturnInProductObem
                    };
                    var resMarket = _marketRepository.OutcomeProduct(market);
                    if (resMarket != "OK")
                    {
                        return Json(new { success = false, message = $"Произошла ошибка: {resMarket}" });
                    }
                    Console.WriteLine("От товара вычитана количество");
                }
                if (returnIn.tempQuantity < returnIn.ReturnInRequest.ReturnInProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = returnIn.ReturnInRequest.ProductNameID,
                        obemProducts = returnIn.ReturnInRequest.ReturnInProductObem - returnIn.tempQuantity
                    };
                    _marketRepository.IncomeProduct(market);
                    Console.WriteLine("К товару прибалена количество");
                }

                var buyProducts = new ReturnInRequest
                {
                    Id = returnIn.ReturnInRequest.Id,
                    ProductNameID = returnIn.ReturnInRequest.ProductNameID,
                    CustomerNameID = returnIn.ReturnInRequest.CustomerNameID,
                    ReturnInProductPrice = returnIn.ReturnInRequest.ReturnInProductPrice,
                    ReturnInProductPriceUSD = returnIn.ReturnInRequest.ReturnInProductPriceUSD,
                    ReturnInProductObem = returnIn.ReturnInRequest.ReturnInProductObem,
                    ReturnInProductSumPrice = returnIn.ReturnInRequest.ReturnInProductSumPrice,
                    ReturnInProductSumPriceUSD = returnIn.ReturnInRequest.ReturnInProductSumPriceUSD,
                    ReturnInProductDate = returnIn.ReturnInRequest.ReturnInProductDate,
                    ReturnInComment = returnIn.ReturnInRequest.ReturnInComment
                };
                _returnIn.Edit(buyProducts);
                return Json(new { success = true, message = "Возврат от покупателя успешно изменена!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var product = _returnIn.GetReturnInProducts().FirstOrDefault(p => p.Id == id); // Используйте ваш сервис или репозиторий
                if (product == null)
                {
                    return Json(new { success = false, message = $"Этот возврат товара от покупателя была удалена" });
                }

                var market = new MarketRequest
                {
                    ProductNameID = product.ProductNameID,
                    obemProducts = product.ReturnInProductObem
                };
                var resMarket = _marketRepository.OutcomeProduct(market);
                if (resMarket == "OK")
                {
                    _returnIn.Delete(product.Id);
                    return Json(new { success = true, message = "Возврат товара от покупателя успешно удалена!" });
                }
                else
                {
                    return Json(new { warning = true, message = resMarket });
                }
            }
            catch (Exception ex)
            {             
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });             
            }
        }
    }
}
