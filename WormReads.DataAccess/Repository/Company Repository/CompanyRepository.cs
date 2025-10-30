using WormReads.Data;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.Company_Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _dbContext;
        public CompanyRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(Company cmp)
        {
            _dbContext.Companies.Update(cmp);
        }
    }
}
