using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
namespace Bookstore.Business.IServices
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        IQueryable<Company> GetAllCompaniesForPaginatedList();

        Task<Company?> GetCompanyAsync(Expression<Func<Company, bool>> predicate);
        Task<bool> AddCompanyAsync(Company category);
        Task<UpdateResult> UpdateAsync(Company category);
        Task DeleteAsync(int id);
    }
}
