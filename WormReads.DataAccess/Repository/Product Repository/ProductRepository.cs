using WormReads.Data;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.Product_Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly AppDbContext _dbContext;
    public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _dbContext = appDbContext;
    }
    public void Update(Product prd)
    {
        _dbContext.Products.Update(prd);
    }
}
