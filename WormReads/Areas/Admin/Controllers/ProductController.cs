using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;
using WormReads.Models.ViewModels;

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

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM();
            var categoriesList = new SelectList(unitOfWork._Category.GetAll(), "Id", "Name");
            if (id == null)
            {
                //ViewBag.Categories = categoriesList;
                 productVM = new ProductVM
                {
                    Categories = categoriesList,
                    product = new Product()
                };
            //return View(productVM);
            }
            else
            {
                productVM.product = unitOfWork._Product.Get(p => p.Id == id);
                productVM.Categories = categoriesList;
                if (productVM.product == null)
                {
                    return NotFound();
                }
            }
                return View(productVM);

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM)
        {
            //ViewBag.Categories = categoriesList; //should be populated again in case invalid model state

            if (unitOfWork._Product.Get(u => u.Id == productVM.product.Id) == null)
            {
                if (ModelState.IsValid)
                {
                    unitOfWork._Product.Add(productVM.product);
                    unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    productVM.Categories = new SelectList(unitOfWork._Category.GetAll(), "Id", "Name");
                    return View(productVM);

                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    unitOfWork._Product.Update(productVM.product);
                    unitOfWork.Save();
                    TempData["success"] = "Product Update successfully";
                    return RedirectToAction("Index");
                }
                productVM.Categories = new SelectList(unitOfWork._Category.GetAll(), "Id", "Name");
                return View(productVM.product);
            }
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
