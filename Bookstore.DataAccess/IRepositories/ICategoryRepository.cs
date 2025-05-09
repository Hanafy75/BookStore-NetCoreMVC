using Bookstore.DataAccess.Models;

namespace Bookstore.DataAccess.IRepositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
    }
}
