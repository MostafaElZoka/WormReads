using Microsoft.AspNetCore.Mvc;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using WormReads.Models.ViewModels;

namespace WormReads.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = unitOfWork._OrderHeader.Get(o => o.Id == id, includes: o => o.User),
                OrderDetails = unitOfWork._OrderDetails.GetAll(o => o.OrderHeaderId == id, includes: o => o.Product)
            };
            return View(orderVM);
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
