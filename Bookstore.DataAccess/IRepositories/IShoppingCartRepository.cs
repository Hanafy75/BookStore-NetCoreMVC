using Bookstore.DataAccess.Models;
using System.Linq.Expressions;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        Task<IEnumerable<ShoppingCart>> GetAllShppingCartForUserAsync(Expression<Func<ShoppingCart,bool>> predicate);
    }
}
