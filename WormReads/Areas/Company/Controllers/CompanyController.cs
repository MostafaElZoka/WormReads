using Microsoft.AspNetCore.Mvc;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;

namespace WormReads.Areas.Company.Controllers
{
    [Area("Company")]
    public class CompanyController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            var companies = unitOfWork._Company.GetAll();
            return View(companies);
        }

        public IActionResult Upsert(int? id)
        {
            var company = new WormReads.Models.Company();
            if (id == null)
            {
                return View(company);
            }
            else
            {
                company = unitOfWork._Company.Get(c => c.Id == id);
                if (company == null)
                {
                    return NotFound();
                }
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Models.Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    unitOfWork._Company.Add(company);
                }
                else
                {
                    unitOfWork._Company.Update(company);
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);

        }
        #region API Calls
        public IActionResult GetAll()
        {
            var companies = unitOfWork._Company.GetAll();
            return Json(new { data = companies });
        }

        public IActionResult Delete(int id)
        {
            if(id != null)
            {
                var company = unitOfWork._Company.Get(c => c.Id == id);
                unitOfWork._Company.Remove(company);
                unitOfWork.Save();
                return Json(new {message = "Company deleted successfully" });
            }
            return Json(new { message = "Error in deleting company" });
        }

        #endregion
    }




}
