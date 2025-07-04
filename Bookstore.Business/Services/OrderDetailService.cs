using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.Business.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync(Expression<Func<OrderDetail, bool>> predicate = null)
        {
            if (predicate is not null) return await _unitOfWork.OrderDetailRepository.GetAll(predicate).ToListAsync();
            return await _unitOfWork.OrderDetailRepository.GetAll().ToListAsync();
        }
        public async Task<OrderDetail?> GetOrderDetailAsync(Expression<Func<OrderDetail, bool>> predicate)
        {
            return await _unitOfWork.OrderDetailRepository.GetAsync(predicate);
        }

        public async Task<bool> AddOrderDetailAsync(OrderDetail orderDetail)
        {
            await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.OrderDetailRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailsIncludeProductAsync(Expression<Func<OrderDetail, bool>> predicate = null)
        {
            return await _unitOfWork.OrderDetailRepository.GetAllOrderDetailsIncludeProduct(predicate);
        }
    }
}

