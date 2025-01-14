using AgroPharm.Interfaces;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AgroPharm.Controllers
{
    public class CustomerController : Controller
    {
        private CustomerRepo _customer;
        public CustomerController(CustomerRepo customer)
        {
            _customer = customer;
        }
        public IActionResult Index()
        {
            try
            {
                var result = _customer.GetCustomers();
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
                var customer = _customer.GetCustomers().FirstOrDefault(p => p.Id == id);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Наименование покупателя не найдена." });
                }
                _customer.DeleteCustomer(id);
                return Json(new { success = true, message = "Наименование покупателя успешно удалёна!" });
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
        public ActionResult Create(Customer customer)
        {            
            try
            {
                if (string.IsNullOrEmpty(customer.customerName))
                {
                    return Json(new { success = false, message = "Наименование покупателя не указано." });
                }

                bool checkName = _customer.CheckCustomerName(customer.customerName);
                if (!checkName)
                {
                    _customer.CreateCustomer(customer);
                    return Json(new { success = true, message = "Наименование покупателя успешно добавлено!" });
                }
                else
                {
                    return Json(new { success = false, message = "Наименование покупателя уже существует." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
            
        }

        public ActionResult Edit(int id)
        {
            var result = _customer.GetCustomers().FirstOrDefault(p => p.Id == id);

            return View("Edit", result);
        }

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            try
            {
                if (string.IsNullOrEmpty(customer.customerName))
                {
                    return Json(new { success = false, message = "Наименование покупателя не указано." });
                }
                var checkName = _customer.CheckCustomerName(customer.customerName);
                if (!checkName)
                {
                    _customer.UpdateCustomer(customer);
                    return Json(new { success = true, message = "Наименование покупателя успешно изменено!" });
                }
                else
                {
                    return Json(new { success = false, message = "Покупатель с таким именем уже существует." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

    }
}
