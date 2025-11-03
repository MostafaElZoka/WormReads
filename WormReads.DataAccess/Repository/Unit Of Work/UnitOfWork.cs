using WormReads.Data;
using WormReads.DataAccess.Repository.Category_Repository;
using WormReads.DataAccess.Repository.Company_Repository;
using WormReads.DataAccess.Repository.Order_Details_Repository;
using WormReads.DataAccess.Repository.Order_Header_Repository;
using WormReads.DataAccess.Repository.Product_Repository;
using WormReads.DataAccess.Repository.Shopping_Cart_Repository;
using WormReads.DataAccess.Repository.User_Rpository;

namespace WormReads.DataAccess.Repository.Unit_Of_Work;

public class UnitOfWork : IUnitOfWork
{
    public ICategoryRepository _Category { get; private set; }
    public IProductRepository _Product { get; private set; }
    public ICompanyRepository _Company { get; private set; }
    public IShoppingCartRepository _ShoppingCart { get; private set; }
    public IUserRpository _User { get; private set; }
    public IOrderHeaderRepository _OrderHeader { get; private set; }
    public IOrderDetailsRepository _OrderDetails { get; private set; }


    private readonly AppDbContext _dbContext;

    public UnitOfWork(ICategoryRepository category,
        IProductRepository productRepository 
        ,ICompanyRepository company,
        IShoppingCartRepository shoppingCartRepository,
        IUserRpository user,
        IOrderDetailsRepository orderDetails,
        IOrderHeaderRepository orderHeader,
        AppDbContext dbContext)
    {
        _Category = category;
        _dbContext = dbContext;
        _Product = productRepository;
        _Company = company;
        _ShoppingCart = shoppingCartRepository;
        _User = user;
        _OrderHeader = orderHeader;
        _OrderDetails = orderDetails;
    }
    public void Save()
    {
        _dbContext.SaveChanges();
    }
}
