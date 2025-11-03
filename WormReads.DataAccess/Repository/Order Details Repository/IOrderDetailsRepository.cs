using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.Order_Details_Repository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetail>
    {
        public void Update(OrderDetail orderDetail);
    }
}
