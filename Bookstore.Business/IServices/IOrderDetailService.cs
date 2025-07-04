using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;

namespace Bookstore.Business.IServices
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync(Expression<Func<OrderDetail, bool>> predicate = null);
        Task<IEnumerable<OrderDetail>> GetAllOrderDetailsIncludeProductAsync(Expression<Func<OrderDetail, bool>> predicate = null);
        Task<OrderDetail?> GetOrderDetailAsync(Expression<Func<OrderDetail, bool>> predicate);
        Task<bool> AddOrderDetailAsync(OrderDetail orderDetail);
        Task DeleteAsync(int id);
    }
}
