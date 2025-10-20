using Microsoft.AspNetCore.Mvc;

namespace WormReads.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
