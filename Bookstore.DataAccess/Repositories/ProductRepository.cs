using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }


        public async Task<bool> IsProductNameExistsAsync(string title, int? excludeId = null)
        {
            return await _context.Products.AnyAsync(c => c.Title.ToLower() == title.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

    }
}
