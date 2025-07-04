using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.DataAccess.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly AppDbContext _context;

        public ApplicationUserRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<ApplicationUser> GetAll()
        {
            return _context.Users.AsNoTracking();
        }

        public async Task<ApplicationUser>? GetAsync(Expression<Func<ApplicationUser, bool>> predicate)
        {
            ApplicationUser? entity = await _context.Users.FirstOrDefaultAsync(predicate);
            return entity;
        }

        public async Task AddAsync(ApplicationUser entity)
        {
            await _context.Users.AddAsync(entity);
        }
        public async Task UpdateAsync(ApplicationUser entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            ApplicationUser? entity = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Category with ID {id} not found.");

            _context.Users.Remove(entity);
        }

        public async Task<ApplicationUser>? GetUserIncludeCompanyAsync(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return await _context.Users.Where(predicate).Include(u=> u.Company).FirstOrDefaultAsync();
        }
    }
}
