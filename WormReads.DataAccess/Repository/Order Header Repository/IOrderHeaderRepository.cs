using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.Order_Header_Repository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    { 
        public void Update(OrderHeader orderHeader);
    }
}
