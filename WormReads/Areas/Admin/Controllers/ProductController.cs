using Microsoft.AspNetCore.Mvc;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;

namespace WormReads.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            var products = unitOfWork._Product.GetAll();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if(ModelState.IsValid)
            {
                unitOfWork._Product.Add(product);
                unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = unitOfWork._Product.Get(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost, ActionName("Edit")]
        public IActionResult EditPost(Product product)
        {
            if (ModelState.IsValid)
            {
                unitOfWork._Product.Update(product);
                unitOfWork.Save();
                TempData["success"] = "Product Update successfully";
                return RedirectToAction("Index");
            }
            return View(product);
        }

       public IActionResult Delete(int id)
        {
            var product = unitOfWork._Product.Get(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {

                unitOfWork._Product.Remove(product);
                unitOfWork.Save();
                TempData["success"] = "Product Deleted successfully";
                return RedirectToAction("Index");
            
        }
    }
}
