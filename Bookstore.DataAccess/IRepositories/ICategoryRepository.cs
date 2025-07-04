using Bookstore.DataAccess.Models;

namespace Bookstore.DataAccess.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null);
    }
}
