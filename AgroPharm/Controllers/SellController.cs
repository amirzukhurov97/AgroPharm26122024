using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgroPharm.Controllers
{
    public class SellController : Controller
    {
        private readonly SellProductRepo _sellProduct;
        private readonly ProductRepo _product;
        private readonly CustomerRepo _customer;
        private readonly MarketRepository _marketRepository;
        public SellController(SellProductRepo sellProduct, ProductRepo product, CustomerRepo customer, MarketRepository marketRepository)
        {
            _sellProduct = sellProduct;
            _product = product;
            _customer = customer;
            _marketRepository = marketRepository;
        }
        public IActionResult Index()
        {
            try
            {
                return View(_sellProduct.GetSellProducts());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

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
                SellRequest = new SellProductRequest()
            };

            return View(model);
        }

        // POST: BuyController/Create
        [HttpPost]
        public ActionResult Create(ViewModel sellProduct)
        {
            try
            {
                if (sellProduct.SellRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (sellProduct.SellRequest.CustomerNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (sellProduct.SellRequest.SellProductPrice == 0 || sellProduct.SellRequest.SellProductPrice == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (sellProduct.SellRequest.SellProductPrice == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                // Сохранение данных в базу
                var sellProducts = new SellProductRequest
                {
                    ProductNameID = sellProduct.SellRequest.ProductNameID,
                    CustomerNameID = sellProduct.SellRequest.CustomerNameID,
                    SellProductPrice = sellProduct.SellRequest.SellProductPrice,
                    SellProductPriceUSD = sellProduct.SellRequest.SellProductPriceUSD,
                    SellProductObem = sellProduct.SellRequest.SellProductObem,
                    SellProductSumPrice = sellProduct.SellRequest.SellProductSumPrice,
                    SellProductSumPriceUSD = sellProduct.SellRequest.SellProductSumPriceUSD,
                    SellProductDate = sellProduct.SellRequest.SellProductDate,
                    SellComment = sellProduct.SellRequest.SellComment
                };
                var market = new MarketRequest
                {
                    ProductNameID = sellProduct.SellRequest.ProductNameID,
                    obemProducts = sellProduct.SellRequest.SellProductObem
                };
                
                var resMarket = _marketRepository.OutcomeProduct(market);
                if (resMarket == "OK")
                {
                    _sellProduct.Create(sellProducts);
                    return Json(new { success = true, message = "Товар успешно продан!" });
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

            var organizations = _customer.GetCustomers().Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.customerName
            }).ToList();

            var itemsRecord = _sellProduct.GetSellProducts().FirstOrDefault(b => b.Id == id);
            ViewModel? model = null;

            if (itemsRecord != null)
            {
                model = new ViewModel
                {
                    Products = products,
                    Organizations = organizations,
                    SellRequest = new SellProductRequest(){
                        Id = id,
                        ProductNameID = itemsRecord?.ProductNameID ?? 0,
                        CustomerNameID = itemsRecord?.CustomerNameID ?? 0,
                        SellProductPrice = itemsRecord?.SellProductPrice ?? 0,
                        SellProductPriceUSD = itemsRecord?.SellProductPriceUSD ?? 0,
                        SellProductObem = itemsRecord?.SellProductObem ?? 0,
                        SellProductSumPrice = itemsRecord?.SellProductSumPrice ?? 0,
                        SellProductSumPriceUSD = itemsRecord?.SellProductSumPriceUSD ?? 0,
                        SellComment = itemsRecord?.SellComment
                    },
                    SellResponse = itemsRecord,

                    tempQuantity = itemsRecord.SellProductObem
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ViewModel sellProduct)
        {
            try
            {
                if (sellProduct.SellRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (sellProduct.SellRequest.CustomerNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (sellProduct.SellRequest.SellProductPrice == 0 || sellProduct.SellRequest.SellProductPriceUSD == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (sellProduct.SellRequest.SellProductObem == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                var producrMarket = _marketRepository.GetMarketList().FirstOrDefault(p => p.ProductNameID == sellProduct.SellRequest.ProductNameID);
                if (sellProduct.tempQuantity > sellProduct.SellRequest.SellProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = sellProduct.SellRequest.ProductNameID,
                        obemProducts = sellProduct.tempQuantity - sellProduct.SellRequest.SellProductObem
                    };
                    _marketRepository.IncomeProduct(market);
                }
                if (sellProduct.tempQuantity < sellProduct.SellRequest.SellProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = sellProduct.SellRequest.ProductNameID,
                        obemProducts = sellProduct.SellRequest.SellProductObem - sellProduct.tempQuantity
                    };
                    var resMarket = _marketRepository.OutcomeProduct(market);
                    if (resMarket != "OK")
                    {
                        return Json(new { success = false, message = $"Произошла ошибка: {resMarket}" });
                    }
                    
                }

                var sellProducts = new SellProductRequest
                {
                    Id = sellProduct.SellRequest.Id,
                    ProductNameID = sellProduct.SellRequest.ProductNameID,
                    CustomerNameID = sellProduct.SellRequest.CustomerNameID,
                    SellProductPriceUSD = sellProduct.SellRequest.SellProductPriceUSD,
                    SellProductPrice = sellProduct.SellRequest.SellProductPrice,
                    SellProductObem = sellProduct.SellRequest.SellProductObem,
                    SellProductSumPrice = sellProduct.SellRequest.SellProductSumPrice,
                    SellProductSumPriceUSD = sellProduct.SellRequest.SellProductSumPriceUSD,
                    SellProductDate = sellProduct.SellRequest.SellProductDate,
                    SellComment = sellProduct.SellRequest.SellComment
                };
                _sellProduct.Edit(sellProducts);
                return Json(new { success = true, message = "Продажа успешно изменена!" });
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
                var product = _sellProduct.GetSellProducts().FirstOrDefault(p => p.Id == id); // Используйте ваш сервис или репозиторий
                if (product == null)
                {
                    return Json(new { success = false, message = $"Эта продажа была удалена" });
                }

                var market = new MarketRequest
                {
                    ProductNameID = product.ProductNameID,
                    obemProducts = product.SellProductObem
                };
                var resMarket = _marketRepository.OutcomeProduct(market);
                if (resMarket == "OK")
                {
                    _sellProduct.Delete(product.Id);
                    return Json(new { success = true, message = "Продажа успешно удалена!" });

                }
                else
                {
                    return Json(new { warning = true, message = resMarket });
                }
            }
            catch (Exception ex)
            {
                {
                    return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
                }
            }
        }

    }
}
