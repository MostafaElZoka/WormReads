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

    public T Get(Expression<Func<T, bool>> filter,bool tracked = false, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query;
        if(tracked)
        {
            query = _dbset;
        }
        else
        {
            query = _dbset.AsNoTracking();
        }

        if (includes != null)
        {
            foreach (var includeProp in includes)
            {
                query = query.Include(includeProp);
            }
        }
        return query.FirstOrDefault(filter);
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, params Expression<Func<T,object>>[] includes)
    {
        IQueryable<T> query = _dbset;
        if (includes != null)
        {
            foreach(var includeProp in includes)
            {
                query = query.Include(includeProp);
            }
        }
        if (filter != null)
            query = query.Where(filter);

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
