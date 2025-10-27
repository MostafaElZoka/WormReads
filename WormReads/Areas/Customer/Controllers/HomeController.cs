using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;

namespace WormReads.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(IUnitOfWork unitOfWork ) : Controller
    {

        public IActionResult Index()
        {
            var products = unitOfWork._Product.GetAll(p => p.Category);
            return View(products);
        }        
        public IActionResult Details(int id)
        {
            var product = unitOfWork._Product.Get(p => p.Id == id,p => p.Category);
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
