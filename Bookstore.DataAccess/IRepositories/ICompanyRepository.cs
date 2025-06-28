using Bookstore.DataAccess.Models;

namespace Bookstore.DataAccess.IRepositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<bool> IsCompanyNameExistsAsync(string name, int? excludeId = null);
    }
}
