using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace AgroPharm.Controllers
{
    public class ReturnOutController : Controller
    {
        private readonly ReturnOutRepo _returnOut;
        private readonly ProductRepo _product;
        private readonly OrganizationRepo _organization;
        private readonly MarketRepository _marketRepository;

        public ReturnOutController(ReturnOutRepo returnOut, ProductRepo product, OrganizationRepo organization, MarketRepository marketRepository)
        {
            _returnOut = returnOut;
            _product = product;
            _organization = organization;
            _marketRepository = marketRepository;
        }
        public IActionResult Index()
        {
            try
            {
                return View(_returnOut.GetReturnOutProducts());
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
        public ActionResult Create()
        {
            var products = _product.GetProducts().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.ProductName
            }).ToList();

            var organizations = _organization.GetOrganizations().Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.OrganizationName
            }).ToList();

            var model = new ViewModel
            {
                Products = products,
                Organizations = organizations,
                ReturnOutRequest = new ReturnOutRequest()
            };

            return View(model);
        }

        // POST: BuyController/Create
        [HttpPost]
        public ActionResult Create(ViewModel returnOutProduct)
        {
            try
            {
                if (returnOutProduct.ReturnOutRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (returnOutProduct.ReturnOutRequest.OrganizationNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (returnOutProduct.ReturnOutRequest.ReturnOutProductPriceUSD == 0 || returnOutProduct.ReturnOutRequest.ReturnOutProductPrice == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (returnOutProduct.ReturnOutRequest.ReturnOutProductObem == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                // Сохранение данных в базу
                var returnOutProducts = new ReturnOutRequest
                {
                    ProductNameID = returnOutProduct.ReturnOutRequest.ProductNameID,
                    OrganizationNameID = returnOutProduct.ReturnOutRequest.OrganizationNameID,
                    ReturnOutProductPrice = returnOutProduct.ReturnOutRequest.ReturnOutProductPrice,
                    ReturnOutProductPriceUSD = returnOutProduct.ReturnOutRequest.ReturnOutProductPriceUSD,
                    ReturnOutProductObem = returnOutProduct.ReturnOutRequest.ReturnOutProductObem,
                    ReturnOutProductSumPrice = returnOutProduct.ReturnOutRequest.ReturnOutProductSumPrice,
                    ReturnOutProductSumPriceUSD = returnOutProduct.ReturnOutRequest.ReturnOutProductSumPriceUSD,
                    ReturnOutProductDate = returnOutProduct.ReturnOutRequest.ReturnOutProductDate,
                    ReturnOutComment = returnOutProduct.ReturnOutRequest.ReturnOutComment
                };
                var market = new MarketRequest
                {
                    ProductNameID = returnOutProduct.ReturnOutRequest.ProductNameID,
                    obemProducts = returnOutProduct.ReturnOutRequest.ReturnOutProductObem
                };

                var resMarket = _marketRepository.OutcomeProduct(market);
                if (resMarket == "OK")
                {
                    _returnOut.Create(returnOutProducts);
                    return Json(new { success = true, message = "Товар успешно возврашён!" });
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

        // GET: BuyController/Edit/5
        public ActionResult Edit(int id)
        {
            var products = _product.GetProducts().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.ProductName
            }).ToList();

            var organizations = _organization.GetOrganizations().Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.OrganizationName
            }).ToList();

            var itemsRecord = _returnOut.GetReturnOutProducts().FirstOrDefault(b => b.Id == id);
            ViewModel? model = null;

            if (itemsRecord != null)
            {
                model = new ViewModel
                {
                    Products = products,
                    Organizations = organizations,
                    ReturnOutRequest = new ReturnOutRequest()
                    {
                        Id = id,
                        ProductNameID = itemsRecord?.ProductNameID ?? 0,
                        OrganizationNameID = itemsRecord?.OrganizationNameID ?? 0,
                        ReturnOutProductPrice = itemsRecord?.ReturnOutProductPrice ?? 0,
                        ReturnOutProductPriceUSD = itemsRecord?.ReturnOutProductPriceUSD ?? 0,
                        ReturnOutProductObem = itemsRecord?.ReturnOutProductObem ?? 0,
                        ReturnOutProductSumPrice = itemsRecord?.ReturnOutSumProductPrice ?? 0,
                        ReturnOutProductSumPriceUSD = itemsRecord?.ReturnOutSumProductPriceUSD ?? 0,
                        ReturnOutComment = itemsRecord?.ReturnOutComment
                    },
                    ReturnOutResponse = itemsRecord,

                    tempQuantity = itemsRecord.ReturnOutProductObem
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ViewModel ReturnOutProduct)
        {
            try
            {
                if (ReturnOutProduct.ReturnOutRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (ReturnOutProduct.ReturnOutRequest.OrganizationNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (ReturnOutProduct.ReturnOutRequest.ReturnOutProductPrice == 0 || ReturnOutProduct.ReturnOutRequest.ReturnOutProductPriceUSD == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (ReturnOutProduct.ReturnOutRequest.ReturnOutProductObem == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                var producrMarket = _marketRepository.GetMarketList().FirstOrDefault(p => p.ProductNameID == ReturnOutProduct.ReturnOutRequest.ProductNameID);
                if (ReturnOutProduct.tempQuantity > ReturnOutProduct.ReturnOutRequest.ReturnOutProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = ReturnOutProduct.ReturnOutRequest.ProductNameID,
                        obemProducts = ReturnOutProduct.tempQuantity - ReturnOutProduct.ReturnOutRequest.ReturnOutProductObem
                    };
                    _marketRepository.IncomeProduct(market);
                }
                if (ReturnOutProduct.tempQuantity < ReturnOutProduct.ReturnOutRequest.ReturnOutProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = ReturnOutProduct.ReturnOutRequest.ProductNameID,
                        obemProducts = ReturnOutProduct.ReturnOutRequest.ReturnOutProductObem - ReturnOutProduct.tempQuantity
                    };
                    var resMarket = _marketRepository.OutcomeProduct(market);
                    if (resMarket != "OK")
                    {
                        return Json(new { success = false, message = $"Произошла ошибка: {resMarket}" });
                    }
                }

                var sellProducts = new ReturnOutRequest
                {
                    Id = ReturnOutProduct.ReturnOutRequest.Id,
                    ProductNameID = ReturnOutProduct.ReturnOutRequest.ProductNameID,
                    OrganizationNameID = ReturnOutProduct.ReturnOutRequest.OrganizationNameID,
                    ReturnOutProductPriceUSD = ReturnOutProduct.ReturnOutRequest.ReturnOutProductPrice,
                    ReturnOutProductPrice = ReturnOutProduct.ReturnOutRequest.ReturnOutProductPriceUSD,
                    ReturnOutProductObem = ReturnOutProduct.ReturnOutRequest.ReturnOutProductObem,
                    ReturnOutProductSumPrice = ReturnOutProduct.ReturnOutRequest.ReturnOutProductSumPrice,
                    ReturnOutProductSumPriceUSD = ReturnOutProduct.ReturnOutRequest.ReturnOutProductSumPriceUSD,
                    ReturnOutProductDate = ReturnOutProduct.ReturnOutRequest.ReturnOutProductDate,
                    ReturnOutComment = ReturnOutProduct.ReturnOutRequest.ReturnOutComment
                };
                _returnOut.Edit(sellProducts);
                return Json(new { success = true, message = "Возврат успешно изменен!" });
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
                var product = _returnOut.GetReturnOutProducts().FirstOrDefault(p => p.Id == id); // Используйте ваш сервис или репозиторий
                if (product == null)
                {
                    return Json(new { success = false, message = $"Этот возврат был удален" });
                }
                var market = new MarketRequest
                {
                    ProductNameID = product.ProductNameID,
                    obemProducts = product.ReturnOutProductObem
                };
                var resMarket = _marketRepository.IncomeProduct(market);
                if (resMarket == "OK")
                {
                    _returnOut.Delete(product.Id);
                    return Json(new { success = true, message = "Возврат товара отменена!" });
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
