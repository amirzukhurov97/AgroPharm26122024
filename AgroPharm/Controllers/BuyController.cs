﻿using AgroPharm.Interfaces;
using AgroPharm.Models.Request;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Ocsp;
using AgroPharm.Models.Response;

namespace AgroPharm.Controllers
{
    public class BuyController : Controller
    {
        private readonly BuyProductRepo _buyProduct;
        private readonly ProductRepo _product;
        private readonly OrganizationRepo _organization;
        private readonly MarketRepository _marketRepository;
        private readonly CurrencyRepo _currencyRepo;

        public BuyController(BuyProductRepo buyProduct, ProductRepo product, OrganizationRepo organization, MarketRepository marketRepository, CurrencyRepo currencyRepo)
        {
            _buyProduct = buyProduct;
            _product = product;
            _organization = organization;
            _marketRepository = marketRepository;
            _currencyRepo = currencyRepo;
        }
        // GET: BuyController
        public ActionResult Index()
        {
            try
            {
                var buyProducts = _buyProduct.GetBuyProducts();
                return View(buyProducts);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
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

            var organizations = _organization.GetOrganizations().Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.OrganizationName
            }).ToList();

            
            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());

            Console.WriteLine($"Текущий курс доллара 1 usd = {currency}");

            var model = new ViewModel
            {
                Products = products,
                Organizations = organizations,
                CurrencyNow = currency,
                BuyProductRequest = new BuyProductRequest()
            };

            return View(model);
        }

        // POST: BuyController/Create
        [HttpPost]
        public ActionResult Create(ViewModel buyProduct)
        {
            try
            {
                if (buyProduct.BuyProductRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (buyProduct.BuyProductRequest.OrganizationNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (buyProduct.BuyProductRequest.BuyProductPrice == 0 || buyProduct.BuyProductRequest.BuyProductPriceUSD == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (buyProduct.BuyProductRequest.BuyProductObem == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                // Сохранение данных в базу
                var buyProducts = new BuyProductRequest
                {
                    ProductNameID = buyProduct.BuyProductRequest.ProductNameID,
                    OrganizationNameID = buyProduct.BuyProductRequest.OrganizationNameID,
                    BuyProductPrice = buyProduct.BuyProductRequest.BuyProductPrice,
                    BuyProductPriceUSD = buyProduct.BuyProductRequest.BuyProductPriceUSD,
                    BuyProductObem = buyProduct.BuyProductRequest.BuyProductObem,
                    BuyProductSumPrice = buyProduct.BuyProductRequest.BuyProductSumPrice,
                    BuyProductSumPriceUSD = buyProduct.BuyProductRequest.BuyProductSumPriceUSD,
                    BuyProductDate = buyProduct.BuyProductRequest.BuyProductDate,
                    BuyComment = buyProduct.BuyProductRequest.BuyComment
                };
                var market = new MarketRequest
                {
                    ProductNameID = buyProduct.BuyProductRequest.ProductNameID,
                    obemProducts = buyProduct.BuyProductRequest.BuyProductObem
                };
                _buyProduct.Create(buyProducts);
                _marketRepository.IncomeProduct(market);
                return Json(new { success = true, message = "Товар успешно куплен!" });
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
            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());

            var itemsRecord = _buyProduct.GetBuyProducts().FirstOrDefault(b => b.Id == id);
            ViewModel? model = null;

            if (itemsRecord != null)
            {
                model = new ViewModel
                {
                    Products = products,
                    Organizations = organizations,
                    BuyProductRequest = new BuyProductRequest()
                    {
                        Id = id,
                        ProductNameID = itemsRecord?.ProductNameID ?? 0,
                        OrganizationNameID = itemsRecord?.OrganizationNameID ?? 0,
                        BuyProductPrice = itemsRecord?.BuyProductPrice ?? 0,
                        BuyProductPriceUSD = itemsRecord?.BuyProductPriceUSD ?? 0,
                        BuyProductObem = itemsRecord?.BuyProductObem ?? 0,
                        BuyProductSumPrice = itemsRecord?.BuyProductSumPrice ?? 0,
                        BuyProductSumPriceUSD = itemsRecord?.BuyProductSumPriceUSD ?? 0,
                        BuyComment = itemsRecord?.BuyComment
                    },
                    BuyProductResponse = itemsRecord,
                    CurrencyNow = currency,
                    tempQuantity = itemsRecord.BuyProductObem
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ViewModel buyProduct)
        {
            try
            {
                if (buyProduct.BuyProductRequest.ProductNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                if (buyProduct.BuyProductRequest.OrganizationNameID == 0)
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                if (buyProduct.BuyProductRequest.BuyProductPrice == 0 || buyProduct.BuyProductRequest.BuyProductPriceUSD == 0)
                {
                    return Json(new { success = false, message = "Цена товара не указана." });
                }
                if (buyProduct.BuyProductRequest.BuyProductObem == 0)
                {
                    return Json(new { success = false, message = "Количество товара не указано." });
                }
                var producrMarket = _marketRepository.GetMarketList().FirstOrDefault(p => p.ProductNameID == buyProduct.BuyProductRequest.ProductNameID);
                if (buyProduct.tempQuantity > buyProduct.BuyProductRequest.BuyProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = buyProduct.BuyProductRequest.ProductNameID,
                        obemProducts = buyProduct.tempQuantity - buyProduct.BuyProductRequest.BuyProductObem
                    };
                    var resMarket = _marketRepository.OutcomeProduct(market);
                    if(resMarket != "OK")
                    {
                        return Json(new { success = false, message = $"Произошла ошибка: {resMarket}" });
                    }
                    Console.WriteLine("От товара вычитана количество");
                }
                if (buyProduct.tempQuantity < buyProduct.BuyProductRequest.BuyProductObem)
                {
                    var market = new MarketRequest
                    {
                        ProductNameID = buyProduct.BuyProductRequest.ProductNameID,
                        obemProducts = buyProduct.BuyProductRequest.BuyProductObem - buyProduct.tempQuantity
                    };
                    _marketRepository.IncomeProduct(market);
                    Console.WriteLine("К товару прибалена количество");
                }

                var buyProducts = new BuyProductRequest
                {
                    Id = buyProduct.BuyProductRequest.Id,
                    ProductNameID = buyProduct.BuyProductRequest.ProductNameID,
                    OrganizationNameID = buyProduct.BuyProductRequest.OrganizationNameID,
                    BuyProductPrice = buyProduct.BuyProductRequest.BuyProductPrice,
                    BuyProductPriceUSD = buyProduct.BuyProductRequest.BuyProductPriceUSD,
                    BuyProductObem = buyProduct.BuyProductRequest.BuyProductObem,
                    BuyProductSumPrice = buyProduct.BuyProductRequest.BuyProductSumPrice,
                    BuyProductSumPriceUSD = buyProduct.BuyProductRequest.BuyProductSumPriceUSD,
                    BuyProductDate = buyProduct.BuyProductRequest.BuyProductDate,
                    BuyComment = buyProduct.BuyProductRequest.BuyComment
                };
                _buyProduct.Edit(buyProducts);
                return Json(new { success = true, message = "Закупка успешно изменена!" });
            }
            catch(Exception ex) 
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var product = _buyProduct.GetBuyProducts().FirstOrDefault(p => p.Id == id); // Используйте ваш сервис или репозиторий
                if (product == null)
                {
                    return Json(new { success = false, message = $"Эта закупка была удалена" });
                }

                var market = new MarketRequest
                {
                    ProductNameID = product.ProductNameID,
                    obemProducts = product.BuyProductObem
                };
                var resMarket = _marketRepository.OutcomeProduct(market);
                if (resMarket == "OK")
                {
                     _buyProduct.Delete(product.Id);                    
                     return Json(new { success = true, message = "Закупка успешно удалена!" });
                    
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
        public IActionResult Report()
        {
            return View();
        }
    }
}
