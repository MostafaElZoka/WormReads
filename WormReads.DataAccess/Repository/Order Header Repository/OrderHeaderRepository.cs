using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WormReads.Data;
using WormReads.DataAccess.Repository.Order_Details_Repository;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.Order_Header_Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext appDb;
        public OrderHeaderRepository(AppDbContext dbContext) : base(dbContext)
        {
            appDb = dbContext;
        }

        public void Update(OrderHeader orderheader)
        {
            appDb.Update(orderheader);
        }

        public void UpdateOrderStatus(int id, string OrderStatus, string? paymentStatus = null)
        {
            var orderFromDb=appDb.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = OrderStatus;
            }
            if(!string.IsNullOrEmpty(paymentStatus))
            {
                orderFromDb.PaymentStatus = paymentStatus;
            }
        }

        public void UpdateStripePropertiesStatuses(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = appDb.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if(!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}
