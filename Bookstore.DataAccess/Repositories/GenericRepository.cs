using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Bookstore.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class,IEntity
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public  IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }
        public  IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).AsNoTracking();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            T? entity = await _context.Set<T>().FirstOrDefaultAsync(predicate);
            return entity;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            T? entity = await _context.Set<T>().FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null) throw new KeyNotFoundException($"{typeof(T).Name} with ID {id} not found.");

            _context.Set<T>().Remove(entity);
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

    }
}
