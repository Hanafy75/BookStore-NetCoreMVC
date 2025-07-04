using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.Repositories
{
    public class OrderRepository : GenericRepository<Order> , IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {

        }


        public async Task<Order> GetOrderIncludeUser(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders.Include(p => p.ApplicationUser).FirstOrDefaultAsync(predicate);
        }

        public async Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = await _context.Orders.FirstOrDefaultAsync(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = await _context.Orders.FirstOrDefaultAsync(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }


    }
}


