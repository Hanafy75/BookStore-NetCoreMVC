using Bookstore.DataAccess.Models;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        Task SaveChangesAsync();
    }
}
