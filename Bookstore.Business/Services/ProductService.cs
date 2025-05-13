using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;

namespace Bookstore.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.ProductRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductAsync(Expression<Func<Product, bool>> predicate)
        {
            return await _unitOfWork.ProductRepository.GetAsync(predicate);
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            //Product with the same name exist
            if (await _unitOfWork.ProductRepository.IsProductNameExistsAsync(product.Title)) return false;

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateResult> UpdateAsync(Product product)
        {
            var oldProduct = await _unitOfWork.ProductRepository.GetAsync(c => c.Id == product.Id);
            if (oldProduct == null) return UpdateResult.NotFound;


            // Check for duplicate name only if the name has changed
            if (!String.Equals(oldProduct.Title, product.Title, StringComparison.OrdinalIgnoreCase) &&
                await _unitOfWork.ProductRepository.IsProductNameExistsAsync(product.Title, product.Id)) return UpdateResult.DuplicateName;


            //check fo no changes
            if(CheckForNoChanges(oldProduct, product) == UpdateResult.NoChanges) return UpdateResult.NoChanges;


            oldProduct.Title = product.Title;
            oldProduct.Description = product.Description;
            oldProduct.Author = product.Author;
            oldProduct.ISBN = product.ISBN;
            oldProduct.ListPrice = product.ListPrice;
            oldProduct.Price = product.Price;
            oldProduct.Price50 = product.Price50;
            oldProduct.Price100 = product.Price100;

            _unitOfWork.ProductRepository.Update(oldProduct);
            await _unitOfWork.SaveChangesAsync();

            return UpdateResult.Updated;
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.ProductRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        private static UpdateResult CheckForNoChanges(Product Old, Product New)
        {
            if (String.Equals(Old.Title, New.Title, StringComparison.OrdinalIgnoreCase)
                && Old.Author == New.Author && Old.Description == New.Description &&
                Old.Price == New.Price && Old.ISBN == New.ISBN && Old.ListPrice== New.ListPrice && Old.Price50 == New.Price50
                && Old.Price100 == New.Price100) return UpdateResult.NoChanges;

            return UpdateResult.Updated;
        }
    }
}
