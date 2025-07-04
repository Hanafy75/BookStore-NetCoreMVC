using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookstore.DataAccess.ViewModels;
namespace Bookstore.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsAsync()
        {
            return await _unitOfWork.ShoppingCartRepository.GetAll().ToListAsync();
        }


        public async Task<IEnumerable<ShoppingCart>> GetAllShoppingForUserCartsAsync(Expression<Func<ShoppingCart, bool>> predicate)
        {
            return await _unitOfWork.ShoppingCartRepository.GetAllShppingCartForUserAsync(predicate);
        }


        public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsIncludeProductsAsync(Expression<Func<ShoppingCart,bool>> predicate)
        {
            return await _unitOfWork.ShoppingCartRepository.GetAll(predicate).Include(sc=> sc.Product).ToListAsync();
        }

        public async Task<ShoppingCart?> GetShoppingCartAsync(Expression<Func<ShoppingCart, bool>> predicate)
        {
            return await _unitOfWork.ShoppingCartRepository.GetAsync(predicate);
        }


        public async Task AddShoppingCartAsync(ShoppingCart shoppingCart, string authUserId)
        {
            //check if there is a shopping cart with the same product
            ShoppingCart cartFromDB = await _unitOfWork.ShoppingCartRepository.GetAsync(u => u.ApplicationUserId == authUserId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDB == null)
                await _unitOfWork.ShoppingCartRepository.AddAsync(shoppingCart);
            else
                cartFromDB.Count += shoppingCart.Count;


            //save changes for add or update.
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShoppingCart shoppingCart)
        {
            _unitOfWork.ShoppingCartRepository.Update(shoppingCart);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.ShoppingCartRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<ShoppingCart> shoppingCarts)
        {
            await _unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCarts);
        }


        public void CalcOrderTotal(ShoppingCartViewModel cartViewModel)
        {
            foreach (var cart in cartViewModel.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                cartViewModel.Order.OrderTotal += (cart.Price * cart.Count);
            }
        }


        private decimal GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }

    }
}

