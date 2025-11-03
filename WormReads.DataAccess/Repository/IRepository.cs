using System.Linq.Expressions;

namespace WormReads.DataAccess.Repository;

public interface IRepository<T> where T : class
{
    public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
    public T Get(Expression<Func<T,bool>> filter, bool trackd = false,params Expression<Func<T, object>>[] includes);
    public void Add(T entity);
    public void Remove(T entity);
    public void RemoveRange(IEnumerable<T> entities);
}
