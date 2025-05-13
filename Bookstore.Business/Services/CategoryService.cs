using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Bookstore.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.CategoryRepository.GetAll().ToListAsync();
        }

        public async Task<Category?> GetCategoryAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _unitOfWork.CategoryRepository.GetAsync(predicate);
        }

        public async Task<bool> AddCategoryAsync(Category category)
        {
            //category with the same name exist
            if (await _unitOfWork.CategoryRepository.IsCategoryNameExistsAsync(category.Name)) return false;

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateResult> UpdateAsync(Category category)
        {
            var oldCategory = await _unitOfWork.CategoryRepository.GetAsync(c=> c.Id==category.Id);
            if (oldCategory == null) return UpdateResult.NotFound;

            // Check for duplicate name only if the name has changed
            if (!String.Equals(oldCategory.Name, category.Name, StringComparison.OrdinalIgnoreCase) &&
                await _unitOfWork.CategoryRepository.IsCategoryNameExistsAsync(category.Name, category.Id)) return UpdateResult.DuplicateName;

            //check fo no changes
            if(String.Equals(oldCategory.Name,category.Name, StringComparison.OrdinalIgnoreCase)
                && oldCategory.DisplayOrder == category.DisplayOrder) return UpdateResult.NoChanges;


            oldCategory.Name = category.Name;
            oldCategory.DisplayOrder = category.DisplayOrder;

            _unitOfWork.CategoryRepository.Update(oldCategory);
            await _unitOfWork.SaveChangesAsync();

            return UpdateResult.Updated;
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.CategoryRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<SelectList> GetSelectListCategories()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAll().Select(c =>
            new
            {
                c.Id,
                c.Name,
            }).ToListAsync();

            return new SelectList(categories, "Id", "Name");
        }
    }
}
