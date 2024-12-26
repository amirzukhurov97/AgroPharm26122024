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
                return View(ex.Message);
            }            
        }
        public IActionResult Delete(int id)
        {
            try
            {
                var success = _porduct.DeleteProduct(id);
                if (success !=null)
                {
                    TempData["DeleteMessage"] = $"{success}";
                    TempData["DeleteSuccess"] = true;
                }
                else
                {
                    TempData["DeleteMessage"] = $"{success}";
                    TempData["DeleteSuccess"] = false;
                }
            }
            catch (Exception ex)
            {
                TempData["DeleteMessage"] = "Произошла ошибка: " + ex.Message;
                TempData["DeleteSuccess"] = false;
            }

            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product product) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkName = _porduct.CheckProductName(product.ProductName);
                    if (checkName)
                    {
                        return RedirectToAction("Warning");
                    }
                    _porduct.CreateProduct(product);
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (HttpIOException)
            {
                return RedirectToAction("ServerError");
            }
            catch (Exception ex)
            {
                return View(ex.Message);
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
                var checkName = _porduct.CheckProductName(product.ProductName);
                if (checkName)
                {
                    return RedirectToAction("Warning");
                }
                if (!ModelState.IsValid)
                {
                    return View(product);
                }
                _porduct.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            catch(HttpIOException){
                return RedirectToAction("ServerError");
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        [Route("/Shared/Warning")]
        public ActionResult Warning()
        {
            return View();
        }
        [Route("/Shared/Error")]
        public ActionResult Error()
        {
            return View();
        }


    }
}
