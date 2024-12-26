using AgroPharm.Interfaces;
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
        public BuyController(BuyProductRepo buyProduct, ProductRepo product, OrganizationRepo organization, MarketRepository marketRepository)
        {
            _buyProduct = buyProduct;
            _product = product;
            _organization = organization;
            _marketRepository = marketRepository;
        }
        // GET: BuyController
        public ActionResult All_BuyProducts()
        {
            return View(_buyProduct.GetBuyProducts());
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

            var model = new BuyProductViewModel
            {
                Products = products,
                Organizations = organizations,
                Request = new BuyProductRequest()
            };

            return View(model);
        }

        // POST: BuyController/Create
        [HttpPost]
        public ActionResult Create(BuyProductViewModel buyProduct)
        {
            try
            {                
                // Сохранение данных в базу
                var buyProducts = new BuyProductRequest
                {
                    ProductNameID = buyProduct.Request.ProductNameID,
                    OrganizationNameID = buyProduct.Request.OrganizationNameID,
                    BuyProductPrice = buyProduct.Request.BuyProductPrice,
                    BuyProductPriceUSD = buyProduct.Request.BuyProductPriceUSD,
                    BuyProductObem = buyProduct.Request.BuyProductObem,
                    BuyProductSumPrice = buyProduct.Request.BuyProductSumPrice,
                    BuyProductSumPriceUSD = buyProduct.Request.BuyProductSumPriceUSD,
                    BuyProductDate = buyProduct.Request.BuyProductDate,
                    BuyComment = buyProduct.Request.BuyComment
                };
                var market = new MarketRequest
                {
                    ProductNameID = buyProduct.Request.ProductNameID,
                    obemProducts = buyProduct.Request.BuyProductObem
                };
                _buyProduct.Create(buyProducts);
                _marketRepository.IncomeProduct(market);
                return RedirectToAction("All_BuyProducts");
            }
            catch
            {
                return RedirectToAction("All_BuyProducts");
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

            var itemsRecord = _buyProduct.GetBuyProducts().FirstOrDefault(b => b.Id == id);
            BuyProductViewModel? model = null;

            if (itemsRecord != null)
            {
                model = new BuyProductViewModel
                {
                    Products = products,
                    Organizations = organizations,
                    Request = new BuyProductRequest()
                    {
                        ProductNameID = itemsRecord?.ProductNameID ?? 0,
                        OrganizationNameID = itemsRecord?.OrganizationNameID ?? 0,
                        BuyProductPrice = itemsRecord?.BuyProductPrice ?? 0,
                        BuyProductPriceUSD = itemsRecord?.BuyProductPriceUSD ?? 0,
                        BuyProductObem = itemsRecord?.BuyProductObem ?? 0,
                        BuyProductSumPrice = itemsRecord?.BuyProductSumPrice ?? 0,
                        BuyProductSumPriceUSD = itemsRecord?.BuyProductSumPriceUSD ?? 0,
                        BuyComment = itemsRecord?.BuyComment
                    },
                    Response = itemsRecord
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BuyProductViewModel buyProduct)
        {
            try
            {
                var buyProducts = new BuyProductRequest
                {
                    ProductNameID = buyProduct.Request.ProductNameID,
                    OrganizationNameID = buyProduct.Request.OrganizationNameID,
                    BuyProductPrice = buyProduct.Request.BuyProductPrice,
                    BuyProductPriceUSD = buyProduct.Request.BuyProductPriceUSD,
                    BuyProductObem = buyProduct.Request.BuyProductObem,
                    BuyProductSumPrice = buyProduct.Request.BuyProductSumPrice,
                    BuyProductSumPriceUSD = buyProduct.Request.BuyProductSumPriceUSD,
                    BuyProductDate = buyProduct.Request.BuyProductDate,
                    BuyComment = buyProduct.Request.BuyComment
                };
                var market = new MarketRequest
                {
                    ProductNameID = buyProduct.Request.ProductNameID,
                    obemProducts = buyProduct.Request.BuyProductObem
                };



                return RedirectToAction("All_Products");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Найти продукт по ID
            var product = _buyProduct.GetBuyProducts().FirstOrDefault(p=>p.Id==id); // Используйте ваш сервис или репозиторий
            if (product == null)
            {
                return NotFound();
            }
            var model = new BuyProductViewModel
            {
                Response = product
            };
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(BuyProductViewModel buyProduct)
        {
            try
            {                
                var market = new MarketRequest
                {
                    ProductNameID = buyProduct.Response.ProductNameID,
                    obemProducts = (double)buyProduct.Response.BuyProductObem
                };
                _buyProduct.Delete(buyProduct.Response.Id);
                _marketRepository.OutcomeProduct(market);
                return RedirectToAction("All_BuyProducts");
            }
            catch
            {
                return View("All_BuyProducts");
            }
        }
    }
}
