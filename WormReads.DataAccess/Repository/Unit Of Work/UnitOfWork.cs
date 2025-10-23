using WormReads.Data;
using WormReads.DataAccess.Repository.Category_Repository;
using WormReads.DataAccess.Repository.Product_Repository;

namespace WormReads.DataAccess.Repository.Unit_Of_Work;

public class UnitOfWork : IUnitOfWork
{
    public ICategoryRepository _Category { get; private set; }
    public IProductRepository _Product { get; private set; }
    private readonly AppDbContext _dbContext;

    public UnitOfWork(ICategoryRepository category,IProductRepository productRepository ,AppDbContext dbContext)
    {
        _Category = category;
        _dbContext = dbContext;
        _Product = productRepository;
    }
    public void Save()
    {
        _dbContext.SaveChanges();
    }
}
