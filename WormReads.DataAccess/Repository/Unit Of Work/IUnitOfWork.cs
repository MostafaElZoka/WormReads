using WormReads.DataAccess.Repository.Category_Repository;
using WormReads.DataAccess.Repository.Company_Repository;
using WormReads.DataAccess.Repository.Order_Details_Repository;
using WormReads.DataAccess.Repository.Order_Header_Repository;
using WormReads.DataAccess.Repository.Product_Repository;
using WormReads.DataAccess.Repository.Shopping_Cart_Repository;
using WormReads.DataAccess.Repository.User_Rpository;

namespace WormReads.DataAccess.Repository.Unit_Of_Work;

public interface IUnitOfWork 
{
    public ICategoryRepository _Category { get; }
    public IProductRepository _Product { get; }
    public ICompanyRepository _Company { get; }
    public IShoppingCartRepository _ShoppingCart { get; }
    public IUserRpository _User { get; }
    public IOrderHeaderRepository _OrderHeader { get; }
    public IOrderDetailsRepository _OrderDetails { get; }
    public void Save();
}
