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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var category = await _context.Categories.Where(c=> c.Id == id).FirstOrDefaultAsync();
            return category ?? throw new Exception("Category not found");
        }
    }
}
