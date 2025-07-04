using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.Common.SD;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _unitOfWork.OrderRepository.GetAll(predicate).ToListAsync();
        }

        public IQueryable<Order> GetAllOrdersForPaginatedListAsync(Expression<Func<Order, bool>> predicate = null)
        {
            if (predicate is not null)
                return _unitOfWork.OrderRepository.GetAll(predicate).Include(o => o.ApplicationUser);

            return _unitOfWork.OrderRepository.GetAll().Include(o => o.ApplicationUser);


        }
        public async Task<Order?> GetOrderAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _unitOfWork.OrderRepository.GetAsync(predicate);
        }
        public async Task<Order?> GetOrderIncludeUserAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _unitOfWork.OrderRepository.GetOrderIncludeUser(predicate);
        }


        public async Task<bool> AddOrderAsync(Order order)
        {
            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        //needs to be updated
        public async Task<UpdateResult> UpdateAsync(Order order, OrderViewModel orderViewModel)
        {
            order.Name = orderViewModel.Order.Name;
            order.PhoneNumber = orderViewModel.Order.PhoneNumber;
            order.StreetAddress = orderViewModel.Order.StreetAddress;
            order.City = orderViewModel.Order.City;
            order.State = orderViewModel.Order.State;
            order.PostalCode = order.PostalCode;
            if (!string.IsNullOrEmpty(order.Carrier))
            {
                order.Carrier = orderViewModel.Order.Carrier;
            }
            if (!string.IsNullOrEmpty(order.TrackingNumber))
            {
                order.Carrier = orderViewModel.Order.TrackingNumber;
            }

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return UpdateResult.Updated;
        }

        public async Task<UpdateResult> UpdateForShipOrderAsync(Order order, OrderViewModel orderViewModel)
        {

            order.TrackingNumber = orderViewModel.Order.TrackingNumber;
            order.Carrier = orderViewModel.Order.Carrier;
            order.OrderStatus = OrderStatus.Shipped;
            order.ShippingDate = DateTime.Now;
            if (order.PaymentStatus == PaymentStatus.DelayedPayment)
            {
                order.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return UpdateResult.Updated;
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.OrderRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            await _unitOfWork.OrderRepository.UpdateStatus(id, orderStatus, paymentStatus);
        }

        public async Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            await _unitOfWork.OrderRepository.UpdateStripePaymentID(id, sessionId, paymentIntentId);
        }
    }
}
