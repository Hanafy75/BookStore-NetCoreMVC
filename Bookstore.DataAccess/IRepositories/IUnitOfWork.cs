using Bookstore.DataAccess.Models;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        Task SaveChangesAsync();
    }
}
