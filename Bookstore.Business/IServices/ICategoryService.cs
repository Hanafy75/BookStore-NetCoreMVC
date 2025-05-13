using Bookstore.Common.Enums;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
namespace Bookstore.Business.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryAsync(Expression<Func<Category,bool>> predicate);
        Task<bool> AddCategoryAsync(Category category);
        Task<UpdateResult> UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
