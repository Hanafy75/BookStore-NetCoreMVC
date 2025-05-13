using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
