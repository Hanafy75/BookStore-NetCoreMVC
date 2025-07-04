using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        public Task<Order> GetOrderIncludeUser(Expression<Func<Order, bool>> predicate);
    }
}
