using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using System.Linq.Expressions;
namespace Bookstore.Business.IServices
{
    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<PaginatedList<ApplicationUser>> GetPaginatedUsersWithRolesAsync(int pageIndex, int pageSize, string searchTerm = null);

        Task<ApplicationUser?> GetUserAsync(Expression<Func<ApplicationUser, bool>> predicate);
        Task<UserViewModel> GetUSerViewModel(string id);

        Task<bool> AddUserAsync(ApplicationUser user);
        public  Task LockOrUnlockUserAsync(string id);
         Task UpdateUserRole(UserViewModel userViewModel);

        Task DeleteAsync(string id);
    }
}
