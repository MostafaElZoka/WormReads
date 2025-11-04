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
                ShoppingCartItems = unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId, includes: c=>c.Product),
                OrderHeader = new()
            };
            foreach(var item in shoppingCart.ShoppingCartItems)
            {
                 item.Price = GetOrderTotal(item);
                shoppingCart.OrderHeader.OrderTotal += (item.Price * item.Count);
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCart = new ShoppingCartVM
            {
                ShoppingCartItems = unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId, includes: c => c.Product),
                OrderHeader = new()
            };
            shoppingCart.OrderHeader.User = unitOfWork._User.Get(u => u.Id == userId);//populate the user navigational prop

            shoppingCart.OrderHeader.City = shoppingCart.OrderHeader.User.City;
            shoppingCart.OrderHeader.PostalCode = shoppingCart.OrderHeader.User.PostalCode;
            shoppingCart.OrderHeader.StreetAddress = shoppingCart.OrderHeader.User.StreetAddress;
            shoppingCart.OrderHeader.State = shoppingCart.OrderHeader.User.State;
            shoppingCart.OrderHeader.PhoneNumber = shoppingCart.OrderHeader.User.PhoneNumber;
            shoppingCart.OrderHeader.Name = shoppingCart.OrderHeader.User.Name;

            foreach (var item in shoppingCart.ShoppingCartItems)
            {
                item.Price = GetOrderTotal(item);
                shoppingCart.OrderHeader.OrderTotal += (item.Price * item.Count);
            }
            return View(shoppingCart);
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
