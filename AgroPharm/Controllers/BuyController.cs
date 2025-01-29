using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using AgroPharm.Models.Buy;
using AgroPharm.Models.Market;

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
                BuyProductRequest = new BuyProductRequest()
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult GenerateProductPdf(ViewModel viewModel)
        {
            decimal? sumTJS=0, sumUSD =0;
            try
            {
                var currencyNow = _currencyRepo.GetLastCurrency();
                IEnumerable<BuyProductResponse>? buyProducts = _buyProduct.GetBuyProducts();

                if (viewModel.ProductNameID != null)
                {
                    buyProducts = buyProducts.Where(b => b.ProductNameID == viewModel.ProductNameID).ToList();
                }
                if (viewModel.OrganizationNameID != null)
                {
                    buyProducts = buyProducts.Where(b => b.OrganizationNameID == viewModel.OrganizationNameID).ToList();
                }
                if(viewModel.StartDate != null)
                {
                    buyProducts = buyProducts.Where(b => b.BuyProductDate > viewModel.StartDate).ToList();
                }
                if (viewModel.EndDate != null) 
                {
                    buyProducts = buyProducts.Where(b => b.BuyProductDate < viewModel.EndDate.Value.AddDays(1)).ToList();
                }
                if (buyProducts.Count() == 0)
                {
                    return Json(new { warning = true, message = $"По выбранным патаметрам нет купленного товара!" });
                }
                buyProducts = buyProducts.OrderBy(b => b.BuyProductDate).ToList();
                using (var ms = new MemoryStream())
                {
                    // Путь к шрифту, поддерживающему кириллицу (Arial в данном случае)
                    BaseFont baseFont = BaseFont.CreateFont("C:/Windows/Fonts/Arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    Font font = new Font(baseFont);

                    Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                    PdfWriter.GetInstance(document, ms);
                    document.Open();
                    Paragraph firstText = new Paragraph($"OAO \"Агро Фарм\"\nтел:+992-92-787-70-01\n{DateTime.Now}\nКурс волют: 1 USD = {currencyNow} TJS.\n\n", font);
                    firstText.Alignment = Element.ALIGN_RIGHT;
                    Paragraph fio = new Paragraph("\nФИО и Подпись продавца: ____________________________ \n\nФИО и Подпись покупателья: __________________________", font);
                    fio.Alignment = Element.ALIGN_LEFT;
                    var titleFont = new Font(baseFont, 14, Font.BOLD);
                    Paragraph title = new Paragraph($"Список купленных товаров\n\n", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(firstText);
                    document.Add(title);

                    // Создаем таблицу
                    PdfPTable table = new PdfPTable(10);
                    PdfPTable resultTable = new PdfPTable(4);
                    resultTable.WidthPercentage = 100;
                    resultTable.SetWidths(new[] { 12.5f, 1.8f, 1.8f, 4f });

                    table.WidthPercentage = 100;
                    table.SetWidths(new[] { 1f, 4f, 3f, 1.5f, 1.5f, 1.5f, 1.8f, 1.8f, 2f, 2f });

                    // Добавляем заголовки столбцов
                    table.AddCell(new PdfPCell(new Phrase("№", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Наименование продукта", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Наименование организации", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Цена TJS", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Цена USD", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Количество, шт", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Сумма TJS", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Сумма USD", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Дата и время", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Комментарий", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });

                    // Добавляем строки с данными
                    foreach (var product in buyProducts)
                    {
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.Id), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.ProductName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.OrganizationName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.BuyProductPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.BuyProductPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.BuyProductObem), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.BuyProductSumPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.BuyProductSumPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.BuyProductDate?.ToString("dd.MM.yyyy") ?? "-", font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.BuyComment), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        sumTJS += product.BuyProductSumPrice;
                        sumUSD += product.BuyProductSumPriceUSD ?? 0;
                    }
                    resultTable.AddCell(new PdfPCell(new Phrase("Итог", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    resultTable.AddCell(new PdfPCell(new Phrase(Convert.ToString(sumTJS),font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    resultTable.AddCell(new PdfPCell(new Phrase(Convert.ToString(sumUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    resultTable.AddCell(new PdfPCell(new Phrase()) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    document.Add(table);
                    document.Add(resultTable);
                    document.Add(fio);
                    document.Close();
                    // Возвращаем PDF файл
                    // Сохранение PDF на диск
                    var filePath = Path.Combine("D:/AgroPharm/Reports", $"BuyProducts_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf");
                    System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
                return Json(new { success = true, message = "Отчёт успешно получен. \nПуть к файлу: D:\\AgroPharm\\Reports" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

    }    
}
