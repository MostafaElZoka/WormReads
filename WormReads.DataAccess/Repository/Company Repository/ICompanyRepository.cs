using WormReads.Models;

namespace WormReads.DataAccess.Repository.Company_Repository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company cmp);
    }
}
