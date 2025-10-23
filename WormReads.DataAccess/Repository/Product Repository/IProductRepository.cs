using WormReads.Models;

namespace WormReads.DataAccess.Repository.Product_Repository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product prd);

}
