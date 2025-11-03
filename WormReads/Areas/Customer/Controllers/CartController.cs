using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;
using WormReads.Models.ViewModels;

namespace WormReads.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCart = new ShoppingCartVM
            {
                ShoppingCartItems = unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId, includes: c=>c.Product)
            };
            foreach(var item in shoppingCart.ShoppingCartItems)
            {
                 item.Price = GetOrderTotal(item);
                shoppingCart.OrderTotal += (item.Price * item.Count);
            }
            return View(shoppingCart);
        }

        private double GetOrderTotal(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
                return shoppingCart.Product.Price;

            else if (shoppingCart.Count <= 100)
                return shoppingCart.Product.Price50;

            else return shoppingCart.Product.Price100;
            
        }
        public IActionResult Summary ()
        {

            return View();
        }
        public IActionResult plus(int id)
        {
            var cartItem = unitOfWork._ShoppingCart.Get(p =>  p.Id == id);
            cartItem.Count++;
            unitOfWork._ShoppingCart.Update(cartItem);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }        
        public IActionResult minus(int id)
        {
            var cartItem = unitOfWork._ShoppingCart.Get(p =>  p.Id == id);
            cartItem.Count--;
            unitOfWork._ShoppingCart.Update(cartItem);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }       
        public IActionResult remove(int id)
        {
            var cartItem = unitOfWork._ShoppingCart.Get(p => p.Id == id);
            unitOfWork._ShoppingCart.Remove(cartItem);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
