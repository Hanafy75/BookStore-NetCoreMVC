using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace Bookstore.Business.IServices
{
    public interface IShoppingCartService
    {
        Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsAsync();
        Task<IEnumerable<ShoppingCart>> GetAllShoppingForUserCartsAsync(Expression<Func<ShoppingCart, bool>> predicate);
        Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsIncludeProductsAsync(Expression<Func<ShoppingCart, bool>> predicate);

        Task<ShoppingCart?> GetShoppingCartAsync(Expression<Func<ShoppingCart, bool>> predicate);
        Task AddShoppingCartAsync(ShoppingCart shoppingCart, string authUserId);
        Task UpdateAsync(ShoppingCart shoppingCart);
        Task DeleteAsync(int id);
        Task RemoveRange(IEnumerable<ShoppingCart>  shoppingCarts);

         void CalcOrderTotal(ShoppingCartViewModel cartViewModel);

    }
}
