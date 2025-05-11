using Bookstore.DataAccess.Models;
namespace Bookstore.Business.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task<bool> UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
