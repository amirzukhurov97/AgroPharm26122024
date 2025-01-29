using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using AgroPharm.Models.Sell;
using AgroPharm.Models.Market;
using AgroPharm.Models.Buy;

namespace AgroPharm.Controllers
{
    public class SellController : Controller
    {
        private readonly SellProductRepo _sellProduct;
        private readonly ProductRepo _product;
        private readonly CustomerRepo _customer;
        private readonly MarketRepository _marketRepository;
        public readonly CurrencyRepo _currencyRepo;
        public SellController(SellProductRepo sellProduct, ProductRepo product, CustomerRepo customer, MarketRepository marketRepository, CurrencyRepo currencyRepo)
        {
            _sellProduct = sellProduct;
            _product = product;
            _customer = customer;
            _marketRepository = marketRepository;
            _currencyRepo = currencyRepo;
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

            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());

            var model = new ViewModel
            {
                Products = products,
                Customers = customers,
                CurrencyNow = currency,
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
            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());
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
                    CurrencyNow=currency,
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
                var resMarket = _marketRepository.IncomeProduct(market);
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

        public IActionResult Report()
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
            decimal? sumTJS = 0, sumUSD = 0;
            try
            {
                var currencyNow = _currencyRepo.GetLastCurrency();
                var sellProducts = _sellProduct.GetSellProducts();

                if (viewModel.ProductNameID != null)
                {
                    sellProducts = sellProducts.Where(b => b.ProductNameID == viewModel.ProductNameID).ToList();
                }
                if (viewModel.OrganizationNameID != null)
                {
                    sellProducts = sellProducts.Where(b => b.CustomerNameID == viewModel.OrganizationNameID).ToList();
                }
                if (viewModel.StartDate != null)
                {
                    sellProducts = sellProducts.Where(b => b.SellProductDate >= viewModel.StartDate).ToList();
                }
                if (viewModel.EndDate != null)
                {
                    sellProducts = sellProducts.Where(b => b.SellProductDate <= viewModel.EndDate.Value.AddDays(1)).ToList();
                }
                if(sellProducts.Count() == 0)
                {
                    return Json(new { warning = true, message = $"По выбранным патаметрам нет проданного товара!" });
                }
                sellProducts = sellProducts.OrderBy(b => b.SellProductDate).ToList();
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
                    Paragraph title = new Paragraph($"Список проданных товаров\n\n", titleFont);
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
                    table.AddCell(new PdfPCell(new Phrase("Наименование покупателя", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Цена TJS", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Цена USD", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Количество, шт", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Сумма TJS", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Сумма USD", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Дата и время", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Комментарий", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });

                    // Добавляем строки с данными
                    foreach (var product in sellProducts)
                    {
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.Id), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.ProductName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.CustomerName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.SellProductPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.SellProductPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.SellProductObem), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.SellProductSumPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.SellProductSumPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.SellProductDate?.ToString("dd.MM.yyyy") ?? "-", font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.SellComment), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        sumTJS += product.SellProductSumPrice;
                        sumUSD += product.SellProductSumPriceUSD ?? 0;
                    }
                    resultTable.AddCell(new PdfPCell(new Phrase("Итог", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    resultTable.AddCell(new PdfPCell(new Phrase(Convert.ToString(sumTJS), font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    resultTable.AddCell(new PdfPCell(new Phrase(Convert.ToString(sumUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    resultTable.AddCell(new PdfPCell(new Phrase()) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    
                    document.Add(table);
                    document.Add(resultTable);
                    document.Add(fio);
                    document.Close();
                    // Возвращаем PDF файл
                    // Сохранение PDF на диск
                    var filePath = Path.Combine("D:/AgroPharm/Reports", $"SellProducts_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");
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
