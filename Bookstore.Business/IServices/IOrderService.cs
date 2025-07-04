using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using System.Linq.Expressions;

namespace Bookstore.Business.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(Expression<Func<Order, bool>> predicate);
        IQueryable<Order> GetAllOrdersForPaginatedListAsync(Expression<Func<Order, bool>> predicate = null);

        Task<Order?> GetOrderAsync(Expression<Func<Order, bool>> predicate);
        public Task<Order?> GetOrderIncludeUserAsync(Expression<Func<Order, bool>> predicate);

        Task<bool> AddOrderAsync(Order order);
        Task<UpdateResult> UpdateAsync(Order order, OrderViewModel orderViewModel);
        Task<UpdateResult> UpdateForShipOrderAsync(Order order, OrderViewModel orderViewModel);

        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        Task DeleteAsync(int id);
    }
}
