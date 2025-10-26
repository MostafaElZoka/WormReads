using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WormReads.Data;

namespace WormReads.DataAccess.Repository;

public class Repository<T>(AppDbContext dbContext) : IRepository<T> where T : class
{
    private DbSet<T> _dbset = dbContext.Set<T>();

    public void Add(T entity)
    {
        _dbset.Add(entity);
    }

    public T Get(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbset;
        if (includes != null)
        {
            foreach (var includeProp in includes)
            {
                query = query.Include(includeProp);
            }
        }
        return query.FirstOrDefault(filter);
    }

    public IEnumerable<T> GetAll(params Expression<Func<T,object>>[] includes)
    {
        IQueryable<T> query = _dbset;
        if (includes != null)
        {
            foreach(var includeProp in includes)
            {
                query = query.Include(includeProp);
            }
        }
        return query.ToList();
    }

    public void Remove(T entity)
    {
        _dbset.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbset.RemoveRange(entities);
    }
}
