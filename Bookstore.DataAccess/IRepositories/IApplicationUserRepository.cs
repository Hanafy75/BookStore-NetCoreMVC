using Bookstore.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IApplicationUserRepository
    {
        IQueryable<ApplicationUser> GetAll();
        Task<ApplicationUser>? GetAsync(Expression<Func<ApplicationUser, bool>> predicate);
        Task<ApplicationUser>? GetUserIncludeCompanyAsync(Expression<Func<ApplicationUser, bool>> predicate);

        Task AddAsync(ApplicationUser entity);
        Task UpdateAsync(ApplicationUser entity);

        Task DeleteAsync(string id);


    }
}
