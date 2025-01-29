using System.Diagnostics;
using AgroPharm.Models;
using AgroPharm.Repositories;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AgroPharm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MarketRepository _market;
        private readonly BuyProductRepo _buyProductRepo;
        private readonly SellProductRepo _sellProductRepo;
        private readonly ReturnInRepo _returnInRepo;
        private readonly ReturnOutRepo _returnOutRepo;
        private readonly CurrencyRepo _currencyRepo;

        public HomeController(ILogger<HomeController> logger, MarketRepository market, BuyProductRepo buyProductRepo, SellProductRepo sellProductRepo, ReturnInRepo returnInRepo, ReturnOutRepo returnOutRepo, CurrencyRepo currencyRepo)
        {
            _logger = logger;
            _market = market;
            _buyProductRepo = buyProductRepo;
            _sellProductRepo = sellProductRepo;
            _returnInRepo = returnInRepo;
            _returnOutRepo = returnOutRepo;
            _currencyRepo = currencyRepo;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var markets = _market.GetMarketList();
                return View(markets);
                 
            }
            catch (MySqlException me)
            {
                return RedirectToAction("ServerError");
                //return Json(new { success = false, message = $"Ошибка при работе с БД: {me.Message}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }
        public IActionResult PrintReport()
        {
            return View();
        }

        [Route("/Shared/ServerError")]
        public IActionResult ServerError()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            var buyProductPriceTotal = _buyProductRepo.GetBuyProductTotal();
            var sellProductTotal = _sellProductRepo.GetSellProductTotal();
            var returnInTotal = _returnInRepo.GetReturnInTotal();
            var returnOutTotal = _returnOutRepo.GetReturnOutTotal();

            var model = new DashboardModel()
            {
                BuyProductPrices = buyProductPriceTotal,
                SellProductPrices = sellProductTotal,
                ReturnInProductPrices = returnInTotal,
                ReturnOutProductPrices = returnOutTotal
            };
            return View(model);
        }

        public ActionResult Report()
        {
            return RedirectToAction("Index");
        }

        public ActionResult GenerateProductPdf()
        {
            try
            {
                var market = _market.GetMarketList();
                using (var ms = new MemoryStream())
                {
                    // Путь к шрифту, поддерживающему кириллицу (Arial в данном случае)
                    BaseFont baseFont = BaseFont.CreateFont("C:/Windows/Fonts/Arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    Font font = new Font(baseFont);

                    Document document = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                    PdfWriter.GetInstance(document, ms);
                    document.Open();
                    Paragraph firstText = new Paragraph($"OAO \"Агро Фарм\"\nтел:+992-92-787-70-01\n{DateTime.Now}", font);
                    firstText.Alignment = Element.ALIGN_RIGHT;
                    var titleFont = new Font(baseFont,14,Font.BOLD);
                    Paragraph title = new Paragraph($"Список товаров на складе\n\n", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(firstText);
                    document.Add(title);

                    // Создаем таблицу
                    PdfPTable table = new PdfPTable(3);

                    table.WidthPercentage = 100;
                    table.SetWidths(new[] { 1f, 5f, 3f });

                    // Добавляем заголовки столбцов
                    table.AddCell(new PdfPCell(new Phrase("№", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Наименование товара", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Количество, шт", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });

                    // Добавляем строки с данными
                    foreach (var product in market)
                    {
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.Id), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(product.ProductName, font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(Convert.ToString(product.ObemProducts), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    }
                    document.Add(table);
                    document.Close();
                    // Возвращаем PDF файл
                    // Сохранение PDF на диск
                    var filePath = Path.Combine("D:/AgroPharm/Reports", $"MarketProducts_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");
                    System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

        public IActionResult AddCurse()
        {
            return View();
        }
    }
}
