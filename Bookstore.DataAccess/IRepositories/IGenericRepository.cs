using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
namespace Bookstore.DataAccess.IRepositories
{
    public interface IGenericRepository<T> where T : class,IEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
    }
}
