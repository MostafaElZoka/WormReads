using WormReads.Models;

namespace WormReads.DataAccess.Repository.Category_Repository;

public interface ICategoryRepository : IRepository<Category>
{
    public void Save();
    public void Update(Category category);
}
