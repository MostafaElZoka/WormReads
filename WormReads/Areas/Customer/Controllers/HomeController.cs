using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WormReads.Application;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;

namespace WormReads.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(IUnitOfWork unitOfWork ) : Controller
    {

        public IActionResult Index()
        {
            //we want to display the cart items count once the user logs in
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                HttpContext.Session.SetInt32(StaticDetails.CartSession,
                    unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId.Value).Count());
            }
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
            if(cartFromDb != null) /// item already exists in DB for the same user
            {
                cartFromDb.Count += cart.Count;
                unitOfWork.Save();
                //unitOfWork._ShoppingCart.Update(cartFromDb);
            }
            else //new item for the user
            {
                unitOfWork._ShoppingCart.Add(cart);
                unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.CartSession,
                    unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId).Count());
            }
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
