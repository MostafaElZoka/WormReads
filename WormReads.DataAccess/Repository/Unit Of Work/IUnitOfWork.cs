using WormReads.DataAccess.Repository.Category_Repository;
using WormReads.DataAccess.Repository.Company_Repository;
using WormReads.DataAccess.Repository.Product_Repository;

namespace WormReads.DataAccess.Repository.Unit_Of_Work;

public interface IUnitOfWork 
{
    public ICategoryRepository _Category { get; }
    public IProductRepository _Product { get; }
    public ICompanyRepository _Company { get; }
    public void Save();
}
