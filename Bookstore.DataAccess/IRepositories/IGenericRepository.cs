using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
namespace Bookstore.DataAccess.IRepositories
{
    public interface IGenericRepository<T> where T : class,IEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
        Task RemoveRange(IEnumerable<T> orders);

    }
}
