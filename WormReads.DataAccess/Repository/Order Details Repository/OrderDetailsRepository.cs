using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WormReads.Data;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.Order_Details_Repository
{
    public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetailsRepository
    {
        private readonly AppDbContext appDb;
        public OrderDetailsRepository(AppDbContext dbContext) : base(dbContext)
        {
            appDb = dbContext;
        }

        public void Update(OrderDetail orderDetail)
        {
            appDb.Update(orderDetail);
        }
    }
}
