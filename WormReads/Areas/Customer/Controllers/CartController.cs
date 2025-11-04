using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WormReads.Application;
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
        [HttpPost]
        public IActionResult Summary (ShoppingCartVM shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ShoppingCartItems = unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId, includes: c => c.Product);
            
            if(!ModelState.IsValid) //if there is validation errors then we need to repopulate the order total price
            { 
                foreach (var item in shoppingCart.ShoppingCartItems)
                {
                    item.Price = GetOrderTotal(item);
                    shoppingCart.OrderHeader.OrderTotal += (item.Price * item.Count);
                }
                return View(shoppingCart);
            }
            shoppingCart.OrderHeader.User = unitOfWork._User.Get(u => u.Id == userId);//populate the user navigational prop we will need it to verify if the user is a company or not
            shoppingCart.OrderHeader.UserId = userId;
            shoppingCart.OrderHeader.OrderDate = DateTime.Now;

            foreach (var item in shoppingCart.ShoppingCartItems)
            {
                item.Price = GetOrderTotal(item);
                shoppingCart.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            if(shoppingCart.OrderHeader.User.CompanyId.GetValueOrDefault() == 0) //this means the user is a regular customer and not a company
            {
                shoppingCart.OrderHeader.OrderStatus = StaticDetails.PaymentStatusPending;
                shoppingCart.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            }
            else //user is company 
            {
                shoppingCart.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
                shoppingCart.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
            }
            unitOfWork._OrderHeader.Add(shoppingCart.OrderHeader);
            unitOfWork.Save();

            foreach (var item in shoppingCart.ShoppingCartItems)
            {
                var details = new OrderDetail
                {
                    OrderHeaderId = shoppingCart.OrderHeader.Id,
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Count = item.Count,
                };
                unitOfWork._OrderDetails.Add(details);              
            }

            unitOfWork.Save();
            return RedirectToAction();
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
