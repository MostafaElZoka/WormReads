using Microsoft.AspNetCore.Mvc;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models.ViewModels;

namespace WormReads.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
        

        #region API
        public IActionResult GetAll(string? status)
        {
            var orders = unitOfWork._OrderHeader.GetAll(includes: o=>o.User);

            if (status != null)
            {
                 orders = unitOfWork._OrderHeader.GetAll(s => s.OrderStatus == status, includes: o => o.User);

            }
            return Json(new { data = orders});
        }
        #endregion
    }
}
