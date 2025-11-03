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
    }
}
