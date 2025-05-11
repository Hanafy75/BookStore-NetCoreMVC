using Bookstore.Business.IServices;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
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

        public async Task<Category?> GetCategoryAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _categoryRepo.GetAsync(predicate);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existingCategory = await _categoryRepo.GetAsync(c=> c.Id==category.Id);
            if (existingCategory == null) return false;

            // Check for duplicate name only if the name has changed
            if (!String.Equals(existingCategory.Name, category.Name, StringComparison.OrdinalIgnoreCase) &&
                await _categoryRepo.IsCategoryNameExistsAsync(category.Name, category.Id)) return false;

            existingCategory.Name = category.Name;
            existingCategory.DisplayOrder = category.DisplayOrder;
            _categoryRepo.Update(existingCategory);
            await _categoryRepo.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            await _categoryRepo.DeleteAsync(id);
            await _categoryRepo.SaveChangesAsync();
        }
    }
}
