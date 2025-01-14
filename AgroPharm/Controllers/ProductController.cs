using AgroPharm.Interfaces;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace AgroPharm.Controllers
{
    public class ProductController : Controller
    {
        private ProductRepo _porduct;
        public ProductController(ProductRepo product)
        {
            _porduct = product;
        }
        public IActionResult Index()
        {
            try
            {
                var result = _porduct.GetProducts();
                return View(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ошибка: {ex.Message}" });
            }            
        }
        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var product = _porduct.GetProducts().FirstOrDefault(p=>p.Id==id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Наименование товара не найдена." });
                }
                _porduct.DeleteProduct(id);
                return Json(new { success = true, message = "Наименование товара успешно удалёна!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ошибка: {ex.Message}" });
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            try
            {
                if (string.IsNullOrEmpty(product.ProductName))
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }

                bool checkName = _porduct.CheckProductName(product.ProductName);
                if (!checkName)
                {
                    _porduct.CreateProduct(product);
                    return Json(new { success = true, message = "Наименование товара успешно добавлено!" });
                }
                else
                {
                    return Json(new { success = false, message = "Наименование товара уже существует." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

        public ActionResult Edit(int id) 
        {
            var result = _porduct.GetProducts().FirstOrDefault(p=>p.Id==id);

            return View("Edit", result);
        }

        [HttpPost]
        public ActionResult Edit(Product product) 
        {
            try
            {
                if (string.IsNullOrEmpty(product.ProductName))
                {
                    return Json(new { success = false, message = "Наименование товара не указано." });
                }
                var checkName = _porduct.CheckProductName(product.ProductName);                
                if (!checkName)
                {
                    _porduct.UpdateProduct(product);
                    return Json(new { success = true, message = "Наименование товара успешно изменено!" });
                }
                else
                {
                    return Json(new { success = false, message = "Наименование товара уже существует." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }
    }
}
