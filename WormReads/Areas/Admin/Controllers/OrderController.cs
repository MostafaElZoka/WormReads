using Microsoft.AspNetCore.Mvc;
using WormReads.DataAccess.Repository.Unit_Of_Work;

namespace WormReads.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            return View();
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
