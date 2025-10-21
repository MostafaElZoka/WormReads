using Microsoft.AspNetCore.Mvc;
using WormReads.Data;
using WormReads.Models;

namespace WormReads.Controllers
{
    public class CategoryController(AppDbContext dbContext) : Controller
    {
        public IActionResult Index()
        {
            var categories = dbContext.Categories.ToList();
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
                dbContext.Categories.Add(category);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();

        }
    }
}
