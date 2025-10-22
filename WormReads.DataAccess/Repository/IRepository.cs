using System.Linq.Expressions;

namespace WormReads.DataAccess.Repository;

public interface IRepository<T> where T : class
{
    public IEnumerable<T> GetAll();
    public T Get(Expression<Func<T,bool>> filter);
    public void Add(T entity);
    public void Remove(T entity);
    public void RemoveRange(IEnumerable<T> entities);
}
