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

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepo.AddAsync(category);
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existingCategory = await _categoryRepo.GetByIdAsync(category.Id);
            if (existingCategory == null) return false;

            // Check for duplicate name only if the name has changed
            if (!String.Equals(existingCategory.Name, category.Name, StringComparison.OrdinalIgnoreCase) &&
                await _categoryRepo.IsCategoryNameExistsAsync(category.Name, category.Id)) return false;

            existingCategory.Name = category.Name;
            existingCategory.DisplayOrder = category.DisplayOrder;
            await _categoryRepo.UpdateAsync(existingCategory);
            return true;
        }
    }
}
