using WormReads.DataAccess.Repository.Category_Repository;

namespace WormReads.DataAccess.Repository.Unit_Of_Work;

public interface IUnitOfWork 
{
    public ICategoryRepository _Category { get; }

    public void Save();
}
