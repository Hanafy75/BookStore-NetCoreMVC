using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>,ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null)
        {
            return await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
            // (!excludeId.HasValue || c.Id != excludeId.Value) => if excludeId not null (has value is true) the ! operator is gonna make the left side false,
            // so it's gonna go for the right side to check if the id is the same or not, but if excludeId is null (has value) the ! operator is gonna make it true,
            // so it become short circuit and it won't check for the ID.
        }
    }
}
