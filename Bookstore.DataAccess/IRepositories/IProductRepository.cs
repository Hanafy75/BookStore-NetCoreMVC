using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<bool> IsProductNameExistsAsync(string name, int? excludeId = null);
        Task<Product> GetProductIncludeCategory(Expression<Func<Product, bool>> predicate);

    }
}
