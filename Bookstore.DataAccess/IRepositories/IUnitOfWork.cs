using Bookstore.DataAccess.Models;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IProductRepository ProductRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IOrderRepository  OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        Task SaveChangesAsync();
    }
}
