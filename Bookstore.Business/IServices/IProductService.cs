using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;

namespace Bookstore.Business.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductAsync(Expression<Func<Product, bool>> predicate);
        Task<bool> AddProductAsync(Product product);
        Task<UpdateResult> UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
