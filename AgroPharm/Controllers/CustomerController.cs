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
            _customer.DeleteCustomer(id);
            return RedirectToAction("Index");
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
                if (ModelState.IsValid)
                {
                    var checkName = _customer.CheckCustomerName(customer.customerName);
                    if (checkName)
                    {
                        return RedirectToAction("Warning");
                    }
                    _customer.CreateCustomer(customer);
                    return RedirectToAction("Index");
                }
                return View(customer);
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
            var result = _customer.GetCustomers().FirstOrDefault(p => p.Id == id);

            return View("Edit", result);
        }

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            try
            {
                var checkName = _customer.CheckCustomerName(customer.customerName);
                if (checkName)
                {
                    return RedirectToAction("Warning", "Product");
                }
                if (!ModelState.IsValid)
                {
                    return View(customer);
                }
                _customer.UpdateCustomer(customer);
                return RedirectToAction("Index");
            }
            catch (HttpIOException)
            {
                return RedirectToAction("ServerError");
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
