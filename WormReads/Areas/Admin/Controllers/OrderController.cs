using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Stripe;
using System.Security.Claims;
using WormReads.Application;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models;
using WormReads.Models.ViewModels;

namespace WormReads.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController(IUnitOfWork unitOfWork) : Controller
    {
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = unitOfWork._OrderHeader.Get(o => o.Id == id, includes: o => o.User),
                OrderDetails = unitOfWork._OrderDetails.GetAll(o => o.OrderHeaderId == id, includes: o => o.Product)
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles =StaticDetails.Admin+", "+StaticDetails.Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderFromDb = unitOfWork._OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            unitOfWork._OrderHeader.Update(orderHeaderFromDb);
            unitOfWork.Save();
            TempData["success"] = "Order was updated successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = StaticDetails.Admin + ", " + StaticDetails.Employee)]
        public IActionResult StartProcessing()
        {
            unitOfWork._OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
            unitOfWork.Save();
            TempData["success"] = "Order was processed successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });

        }
        [HttpPost]
        [Authorize(Roles = StaticDetails.Admin + ", " + StaticDetails.Employee)]
        public IActionResult StartShipping()
        {
            var orderFromDb = unitOfWork._OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id);
            orderFromDb.OrderStatus = StaticDetails.StatusShipped;
            orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderFromDb.ShippingDate = DateTime.Now;
            //if a company is making a delayed payment we delay the payment due date by 30 days
            if(orderFromDb.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                orderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            unitOfWork._OrderHeader.Update(orderFromDb);
            unitOfWork.Save();
            TempData["success"] = "Order was shipped successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });

        }
        [HttpPost]
        [Authorize(Roles = StaticDetails.Admin + ", " + StaticDetails.Employee)]
        public IActionResult CancelOrder()
        {
            var order = unitOfWork._OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id);
            //if customer already paid then we refund him using stripe
            if(order.PaymentStatus == StaticDetails.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId
                };
                
                var service = new RefundService(); //This creates a RefundService object from the Stripe SDK. Think of it as the module responsible for talking to Stripe’s refund API.

                var refund = service.Create(options); //This actually sends the refund request to Stripe using the parameters you defined in options.

                unitOfWork._OrderHeader.UpdateOrderStatus(order.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else
            {
                unitOfWork._OrderHeader.UpdateOrderStatus(order.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
            }
            unitOfWork.Save();
            TempData["success"] = "Order was cancelled successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }

        #region API
        public IActionResult GetAll(string? status)
        {
            IEnumerable<OrderHeader> orders;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //if user is an admin or emp then he can only view his orders
            if(User.IsInRole(StaticDetails.Admin) || User.IsInRole(StaticDetails.Employee))
            {
                orders = unitOfWork._OrderHeader.GetAll(includes: o=>o.User);   
            }
            else //else manage ur own orders
            {
                orders = unitOfWork._OrderHeader.GetAll(filter: o=>o.UserId == userId, includes: o=> o.User);
            }
            if (status != null && (User.IsInRole(StaticDetails.Admin) || User.IsInRole(StaticDetails.Employee))) //if the user clicked on a certain status then get the orders of that one
            {
                orders = unitOfWork._OrderHeader.GetAll(s => s.OrderStatus == status, includes: o => o.User);

            }
            if (status != null && !(User.IsInRole(StaticDetails.Admin) || User.IsInRole(StaticDetails.Employee))) //if the user clicked on a certain status then get the orders of that one
            {
                orders = unitOfWork._OrderHeader.GetAll(s => (s.OrderStatus == status && s.UserId == userId), includes: o => o.User);

            }
            return Json(new { data = orders});
        }
        #endregion
    }
}
