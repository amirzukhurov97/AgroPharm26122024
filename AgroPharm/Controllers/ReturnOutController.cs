using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using AgroPharm.Models.ReturnOut;
using AgroPharm.Models.Market;
using AgroPharm.Models.Buy;

namespace AgroPharm.Controllers
{
    public class ReturnOutController : Controller
    {
        private readonly ReturnOutRepo _returnOut;
        private readonly ProductRepo _product;
        private readonly OrganizationRepo _organization;
        private readonly MarketRepository _marketRepository;
        public readonly CurrencyRepo _currencyRepo; 

        public ReturnOutController(ReturnOutRepo returnOut, ProductRepo product, OrganizationRepo organization, MarketRepository marketRepository, CurrencyRepo currencyRepo)
        {
            _returnOut = returnOut;
            _product = product;
            _organization = organization;
            _marketRepository = marketRepository;
            _currencyRepo = currencyRepo;
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

            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());

            var model = new ViewModel
            {
                Products = products,
                Organizations = organizations,
                CurrencyNow = currency,
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

            var currency = Convert.ToDecimal(_currencyRepo.GetLastCurrency());
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
                    CurrencyNow = currency,
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
            decimal? sumTJS = 0, sumUSD = 0;
            try
            {
                var currencyNow = _currencyRepo.GetLastCurrency();
                var    returnOutProducts = _returnOut.GetReturnOutProducts();

                if (viewModel.ProductNameID != null)
                {
                    returnOutProducts = returnOutProducts.Where(b => b.ProductNameID == viewModel.ProductNameID).ToList();
                }
                if (viewModel.OrganizationNameID != null)
                {
                    returnOutProducts = returnOutProducts.Where(b => b.OrganizationNameID == viewModel.OrganizationNameID).ToList();
                }
                if (viewModel.StartDate != null)
                {
                    returnOutProducts = returnOutProducts.Where(b => b.ReturnOutProductDate >= viewModel.StartDate).ToList();
                }
                if (viewModel.EndDate != null)
                {
                    returnOutProducts = returnOutProducts.Where(b => b.ReturnOutProductDate <= viewModel.EndDate.Value.AddDays(1)).ToList();
                }
                if (returnOutProducts.Count() == 0)
                {
                    return Json(new { warning = true, message = $"По выбранным патаметрам нет купленного товара!" });
                }
                returnOutProducts = returnOutProducts.OrderBy(b => b.ReturnOutProductDate).ToList();
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
                    Paragraph title = new Paragraph($"Список возврата организацию\n\n", titleFont);
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
                    foreach (var product in returnOutProducts)
                    {
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.Id), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.ProductName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.OrganizationName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnOutProductPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnOutProductPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnOutProductObem), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnOutSumProductPrice), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnOutSumProductPriceUSD), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.ReturnOutProductDate?.ToString("dd.MM.yyyy") ?? "-", font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ReturnOutComment), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        sumTJS += product.ReturnOutSumProductPrice;
                        sumUSD += product.ReturnOutSumProductPriceUSD ?? 0;
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
                    var filePath = Path.Combine("D:/AgroPharm/Reports", $"ReturnOutProducts_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");
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
