using AgroPharm.Interfaces;
using AgroPharm.Models;
using AgroPharm.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AgroPharm.Controllers
{
    public class OrganizationController : Controller
    {
        private OrganizationRepo _organization;
        public OrganizationController(OrganizationRepo organization)
        {
            _organization = organization;
        }
        public IActionResult Index()
        {
            try
            {
                var result = _organization.GetOrganizations();
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
                var org = _organization.GetOrganizations().FirstOrDefault(p => p.Id == id);
                if (org == null)
                {
                    return Json(new { success = false, message = "Наименование организации не найдена." });
                }
                _organization.DeleteOrganization(id);
                return Json(new { success = true, message = "Наименование организации успешно удалёна!" });
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
        public ActionResult Create(Organization organization)
        {
            try
            {
                if (string.IsNullOrEmpty(organization.OrganizationName))
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }

                bool checkName = _organization.CheckOrganziationName(organization.OrganizationName);
                if (!checkName)
                {
                    _organization.CreateOrganization(organization);
                    return Json(new { success = true, message = "Наименование организация успешно добавлено!" });
                }
                else
                {
                    return Json(new { success = false, message = "Наименование организации уже существует." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }
        }

        public ActionResult Edit(int id)
        {
            var result = _organization.GetOrganizations().FirstOrDefault(p => p.Id == id);

            return View("Edit", result);
        }

        [HttpPost]
        public ActionResult Edit(Organization organization)
        {
            try
            {
                if (string.IsNullOrEmpty(organization.OrganizationName))
                {
                    return Json(new { success = false, message = "Наименование организации не указано." });
                }
                var checkName = _organization.CheckOrganziationName(organization.OrganizationName);
                if (!checkName)
                {
                    _organization.UpdateOrganization(organization);
                    return Json(new { success = true, message = "Наименование организации успешно изменено!" });
                }
                else
                {
                    return Json(new { success = false, message = "Наименование организации уже существует." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
            }

        }
    }
}
