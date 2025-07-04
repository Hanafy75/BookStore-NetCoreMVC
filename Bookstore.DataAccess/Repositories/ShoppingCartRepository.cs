using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.DataAccess.Repositories
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<ShoppingCart>> GetAllShppingCartForUserAsync(Expression<Func<ShoppingCart, bool>> predicate)
        {
            return await _context.shoppingCarts.Where(predicate).ToListAsync();
        }
    }
}
