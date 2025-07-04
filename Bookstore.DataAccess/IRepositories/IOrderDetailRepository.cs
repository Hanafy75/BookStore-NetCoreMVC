using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.IRepositories
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetAllOrderDetailsIncludeProduct(Expression<Func<OrderDetail, bool>> predicate);

    }
}
