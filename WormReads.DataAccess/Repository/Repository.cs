using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WormReads.Data;

namespace WormReads.DataAccess.Repository;

internal class Repository<T>(AppDbContext dbContext) : IRepository<T> where T : class
{
    private DbSet<T> _dbset = dbContext.Set<T>();

    public void Add(T entity)
    {
        _dbset.Add(entity);
        dbContext.SaveChanges();
    }

    public T Get(Expression<Func<T, bool>> filter)
    {
        return _dbset.FirstOrDefault(filter);
    }

    public IEnumerable<T> GetAll()
    {
        return _dbset.ToList();
    }

    public void Remove(T entity)
    {
        _dbset.Remove(entity);
        dbContext.SaveChanges();
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbset.RemoveRange(entities);
        dbContext.SaveChanges();
    }
}
