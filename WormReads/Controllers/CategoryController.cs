using Microsoft.AspNetCore.Mvc;
using WormReads.Data;

namespace WormReads.Controllers
{
    public class CategoryController(AppDbContext dbContext) : Controller
    {
        public IActionResult Index()
        {
            var categories = dbContext.Categories.ToList();
            return View(categories);
        }
    }
}
