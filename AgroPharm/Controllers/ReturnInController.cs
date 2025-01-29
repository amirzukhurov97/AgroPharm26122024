using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using AgroPharm.Models.ReturnIn;
using AgroPharm.Models.Market;
using AgroPharm.Models.Buy;

namespace AgroPharm.Controllers
{
    public class ReturnInController : Controller
    {
        private readonly ReturnInRepo _returnIn;
        private readonly ProductRepo _product;
        private readonly CustomerRepo _customer;
        private readonly MarketRepository _marketRepository;
        public readonly CurrencyRepo _currencyRepo;

        public ReturnInController(ReturnInRepo returnIn, ProductRepo product, CustomerRepo customer, MarketRepository marketRepository, CurrencyRepo currencyRepo)
        {
            _returnIn = returnIn;
            _product = product;
            _customer = customer;
            _marketRepository = marketRepository;
            _currencyRepo = currencyRepo;
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

            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());

            var model = new ViewModel
            {
                Products = products,
                Customers = customers,
                CurrencyNow = currency,
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
            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());
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
                    CurrencyNow = currency,
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
                var returnInProducts = _returnIn.GetReturnInProducts();

                if (viewModel.ProductNameID != null)
                {
                    returnInProducts = returnInProducts.Where(b => b.ProductNameID == viewModel.ProductNameID).ToList();
                }
                if (viewModel.OrganizationNameID != null)
                {
                    returnInProducts = returnInProducts.Where(b => b.CustomerNameID == viewModel.OrganizationNameID).ToList();
                }
                if (viewModel.StartDate != null)
                {
                    returnInProducts = returnInProducts.Where(b => b.ReturnInProductDate >= viewModel.StartDate).ToList();
                }
                if (viewModel.EndDate != null)
                {
                    returnInProducts = returnInProducts.Where(b => b.ReturnInProductDate <= viewModel.EndDate.Value.AddDays(1)).ToList();
                }
                if (returnInProducts.Count() == 0)
                {
                    return Json(new { warning = true, message = $"По выбранным патаметрам нет купленного товара!" });
                }
                returnInProducts = returnInProducts.OrderBy(b => b.ReturnInProductDate).ToList();
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
                    Paragraph title = new Paragraph($"Список возврата от покупателя\n\n", titleFont);
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
                    foreach (var product in returnInProducts)
                    {
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.Id), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.ProductName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.CustomerName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnInProductPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnInProductPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnInProductObem), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnInSumProductPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnInSumProductPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.ReturnInProductDate?.ToString("dd.MM.yyyy") ?? "-", font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnInComment), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        sumTJS += product.ReturnInSumProductPrice;
                        sumUSD += product.ReturnInSumProductPriceUSD ?? 0;
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
                    var filePath = Path.Combine("D:/AgroPharm/Reports", $"ReturnInProducts_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf");
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
