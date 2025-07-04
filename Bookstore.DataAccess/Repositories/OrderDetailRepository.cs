using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.DataAccess.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>,IOrderDetailRepository
    {
        public OrderDetailRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailsIncludeProduct(Expression<Func<OrderDetail, bool>> predicate)
        {
            return await _context.OrderDetails.Include(Od => Od.Product).Where(predicate).ToListAsync();
        }
    }
}

