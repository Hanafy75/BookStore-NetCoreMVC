using Bookstore.Business.IServices;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
namespace Bookstore.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepo.GetByIdAsync(id);
        }
    }
}
