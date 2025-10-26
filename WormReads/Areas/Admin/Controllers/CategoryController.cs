using Microsoft.AspNetCore.Mvc;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;

namespace WormReads.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            var categories = unitOfWork._Category.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid)
            {
                unitOfWork._Category.Add(category);
                unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int id)
        {
            var category = unitOfWork._Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork._Category.Update(category);
                unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(category);

        }

        public IActionResult Delete(int id)
        {
            var category = unitOfWork._Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var category = unitOfWork._Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            unitOfWork._Category.Remove(category);
            unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
