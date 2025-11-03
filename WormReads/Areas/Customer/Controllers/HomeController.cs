using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
            var products = unitOfWork._Product.GetAll(includes: p => p.Category);
            return View(products);
        }        
        public IActionResult Details(int productId)
        {
            var shoppingCart = new ShoppingCart()
            {
                Product = unitOfWork._Product.Get(p => p.Id == productId,includes: p => p.Category),
                ProductId = productId,
                Count = 1
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.UserId = userId;
            var cartFromDb = unitOfWork._ShoppingCart.Get(c => c.UserId == userId && c.ProductId == cart.ProductId);

            if(cartFromDb != null) ///cart item already exists in DB for the same user
            {
                cartFromDb.Count += cart.Count;
                //unitOfWork._ShoppingCart.Update(cartFromDb);
            }
            else //new cart item for the user
            {
                unitOfWork._ShoppingCart.Add(cart);
            }
            unitOfWork.Save();
            TempData["success"] = "Item Added To Cart Successfully";
            return RedirectToAction(nameof(Index));
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
