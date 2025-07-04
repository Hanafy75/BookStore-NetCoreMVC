using Bookstore.Business.IServices;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
namespace Bookstore.Business.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _unitOfWork.ApplicationUserRepository.GetAll().ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserAsync(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return await _unitOfWork.ApplicationUserRepository.GetAsync(predicate);
        }

        public async Task<bool> AddUserAsync(ApplicationUser user)
        {
            await _unitOfWork.ApplicationUserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(string id)
        {
            await _unitOfWork.ApplicationUserRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
