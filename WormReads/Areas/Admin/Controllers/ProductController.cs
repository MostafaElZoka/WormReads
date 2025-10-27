using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;
using WormReads.Models.ViewModels;

namespace WormReads.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : Controller
    {
        public IActionResult Index()
        {
            var products = unitOfWork._Product.GetAll(p => p.Category);
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM();
            var categoriesList = new SelectList(unitOfWork._Category.GetAll(), "Id", "Name");
            if (id == null)
            {
                //create
                 productVM = new ProductVM
                {
                    Categories = categoriesList,
                    product = new Product()
                };
            }
            else
            {
                //update
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
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = new SelectList(unitOfWork._Category.GetAll(), "Id", "Name");
                return View(productVM);
            }

            string wwwRootPath = webHostEnvironment.WebRootPath;

            if (file != null)
            {
                string productPath = Path.Combine(wwwRootPath, "images", "product");
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(productVM.product.ImageUrl))
                {
                    string oldImagePath = Path.Combine(wwwRootPath, productVM.product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                // Save new image
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productVM.product.ImageUrl = $@"\images\product\{fileName}";
            }

            if (productVM.product.Id == 0)
            {
                unitOfWork._Product.Add(productVM.product);
                TempData["success"] = "Product created successfully";
            }
            else
            {
                unitOfWork._Product.Update(productVM.product);
                TempData["success"] = "Product updated successfully";
            }

            unitOfWork.Save();
            return RedirectToAction("Index");
        }


        //public IActionResult Delete(int id)
        //{
        //    var product = unitOfWork._Product.Get(p => p.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}
        //[HttpPost]
        //public IActionResult Delete(Product product)
        //{

        //        unitOfWork._Product.Remove(product);
        //        unitOfWork.Save();
        //        TempData["success"] = "Product Deleted successfully";
        //        return RedirectToAction("Index");
            
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = unitOfWork._Product.GetAll(p => p.Category);
            return Json(new { data = products });
        }

        public IActionResult Delete(int? id)
        {
            var product = unitOfWork._Product.Get(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "error in deleting product" });
            }
            string wwwRootPath = webHostEnvironment.WebRootPath;
            if(product.ImageUrl != null)
            {
                string oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }

            unitOfWork._Product.Remove(product);
            unitOfWork.Save();
            return Json(new { success = true, message = "product deleted successfully" });
        }
        #endregion
    }
}
