using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AgroPharm.Controllers
{
    public class OrganizationController : Controller
    {
        private OrganizationRepo _porduct;
        public OrganizationController(OrganizationRepo product)
        {
            _porduct = product;
        }
        public IActionResult Index()
        {
            try
            {
                var result = _porduct.GetOrganizations();
                return View(result);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
        public IActionResult Delete(int id)
        {
            _porduct.DeleteOrganization(id);
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Organization organization)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkName = _porduct.CheckOrganziationName(organization.OrganizationName);
                    if (checkName)
                    {
                        return RedirectToAction("Warning");
                    }
                    _porduct.CreateOrganization(organization);
                    return RedirectToAction("Index");
                }
                return View(organization);
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
            var result = _porduct.GetOrganizations().FirstOrDefault(p => p.Id == id);

            return View("Edit", result);
        }

        [HttpPost]
        public ActionResult Edit(Organization organization)
        {
            try
            {
                var checkName = _porduct.CheckOrganziationName(organization.OrganizationName);
                if (checkName)
                {
                    return RedirectToAction("Warning", "Product");
                }
                if (!ModelState.IsValid)
                {
                    return View(organization);
                }
                _porduct.UpdateOrganization(organization);
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
