using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace Bookstore.Business.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductAsync(Expression<Func<Product, bool>> predicate);
        public  Task<Product?> GetProductWithCategoryAsync(Expression<Func<Product, bool>> predicate);

        Task<bool> AddProductAsync(Product product, IFormFile imageFile, string webRootPath);
        Task<UpdateResult> UpdateAsync(Product product, IFormFile imageFile, string webRootPath);
        Task DeleteAsync(int id, string webRootPath);
        public IQueryable<ProductIndexViewModel> GetAllProductsIncludeCategoryName();
        //Task<string> SetImageUrlAsync(IFormFile imageFile, string webRootPath);
    }
}
