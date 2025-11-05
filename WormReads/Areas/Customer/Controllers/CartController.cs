using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
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
                ShoppingCartItems = unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId, includes: c => c.Product),
                OrderHeader = new()
            };
            foreach (var item in shoppingCart.ShoppingCartItems)
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
        public IActionResult Summary()
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
        public IActionResult Summary(ShoppingCartVM shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.OrderHeader.UserId = userId;

            if (!ModelState.IsValid) //if there is validation errors then we need to repopulate the order total price
            {
                shoppingCart.ShoppingCartItems = unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId, includes: c => c.Product);

                foreach (var item in shoppingCart.ShoppingCartItems)
                {
                    item.Price = GetOrderTotal(item);
                    shoppingCart.OrderHeader.OrderTotal += (item.Price * item.Count);
                }
                return View(shoppingCart);
            }

            shoppingCart.ShoppingCartItems = unitOfWork._ShoppingCart.GetAll(c => c.UserId == userId, includes: c => c.Product);

            User user = unitOfWork._User.Get(u => u.Id == userId);//populate the user navigational prop we will need it to verify if the user is a company or not
            shoppingCart.OrderHeader.UserId = userId;
            shoppingCart.OrderHeader.OrderDate = DateTime.Now;

            foreach (var item in shoppingCart.ShoppingCartItems)
            {
                item.Price = GetOrderTotal(item);
                shoppingCart.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            if (user.CompanyId.GetValueOrDefault() == 0) //this means the user is a regular customer and not a company
            {
                // we change the status 
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
            unitOfWork.Save(); //to save orderdetails 

            if(user.CompanyId.GetValueOrDefault() ==0)
            {
                //if user is a customer then he must pay immediately
                //stripe logic
                var domain = "https://localhost:7119";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"/customer/cart/OrderConfirmation?id={shoppingCart.OrderHeader.Id}",
                    CancelUrl = domain + "/customer/cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    //{
                    //    new Stripe.Checkout.SessionLineItemOptions
                    //    {
                    //        Price = "price_1MotwRLkdIwHu7ixYcPLm5uZ",
                    //        Quantity = 2,
                    //    },
                    //},
                    Mode = "payment",

                };
                foreach (var item in shoppingCart.ShoppingCartItems)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {

                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);

                unitOfWork._OrderHeader.UpdateStripePropertiesStatuses(shoppingCart.OrderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Save();
                return Redirect(session.Url);

            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCart.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            var orderId = id;
            var orderHeader = unitOfWork._OrderHeader.Get(o => o.Id == id);

            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid") //the user paid and transaction completed successfully
            {
                unitOfWork._OrderHeader.UpdateStripePropertiesStatuses(orderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork._OrderHeader.UpdateOrderStatus(orderHeader.Id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                unitOfWork.Save();
            }
            var shoppingItems = unitOfWork._ShoppingCart.GetAll(s => s.UserId == orderHeader.UserId);
            unitOfWork._ShoppingCart.RemoveRange(shoppingItems);
            unitOfWork.Save();

            return View(orderId);
        }
        public IActionResult plus(int id)
        {
            var cartItem = unitOfWork._ShoppingCart.Get(p => p.Id == id);
            cartItem.Count++;
            unitOfWork._ShoppingCart.Update(cartItem);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int id)
        {
            var cartItem = unitOfWork._ShoppingCart.Get(p => p.Id == id);
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
