using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
namespace Bookstore.Business.IServices
{
    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserAsync(Expression<Func<ApplicationUser, bool>> predicate);
        Task<bool> AddUserAsync(ApplicationUser user);
        Task DeleteAsync(string id);
    }
}
