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
        public void UpdateOrderStatus(int id,string OrderStatus, string? paymentStatus = null);
        public void UpdateStripePropertiesStatuses(int id, string sessionId, string paymentIntentId); //will be used to update the sessionId and paymentIntentId once we consume stripe
    }
}
