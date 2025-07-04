using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<Product> GetProductIncludeCategory(Expression<Func<Product, bool>> predicate)
        {
            return await _context.Products.Include(p=>p.Category).FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> IsProductNameExistsAsync(string title, int? excludeId = null)
        {
            return await _context.Products.AnyAsync(c => c.Title.ToLower() == title.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
        }



    }
}
