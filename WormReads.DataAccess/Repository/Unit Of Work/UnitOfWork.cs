using WormReads.Data;
using WormReads.DataAccess.Repository.Category_Repository;

namespace WormReads.DataAccess.Repository.Unit_Of_Work;

public class UnitOfWork : IUnitOfWork
{
    public ICategoryRepository _Category { get; private set; }
    private readonly AppDbContext _dbContext;

    public UnitOfWork(ICategoryRepository category, AppDbContext dbContext)
    {
        _Category = category;
        _dbContext = dbContext;
    }
    public void Save()
    {
        _dbContext.SaveChanges();
    }
}
