using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
            return await _unitOfWork.ProductRepository.GetAll().ToListAsync();
        }

        public IQueryable<ProductIndexViewModel> GetAllProductsIncludeCategoryName()
        {
            return _unitOfWork.ProductRepository.GetAll().Include(c => c.Category)
                .Select(pc =>
                new ProductIndexViewModel
                {
                    Id = pc.Id,
                    Title = pc.Title,
                    Author = pc.Author,
                    ISBN = pc.ISBN,
                    ListPrice = pc.ListPrice,
                    CategoryName = pc.Category!.Name
                });
        }

        public async Task<Product?> GetProductAsync(Expression<Func<Product, bool>> predicate)
        {
            return await _unitOfWork.ProductRepository.GetAsync(predicate);
        }

        public async Task<Product?> GetProductWithCategoryAsync(Expression<Func<Product, bool>> predicate)
        {
            //var product = await _unitOfWork.ProductRepository.GetAsync(predicate);
            //product.Category = await _unitOfWork.CategoryRepository.GetAsync(c => c.Id == product.CategoryId);
            return await _unitOfWork.ProductRepository.GetProductIncludeCategory(predicate);
        }

        public async Task<bool> AddProductAsync(Product product, IFormFile imageFile, string webRootPath)
        {
            //Product with the same name exist
            if (await _unitOfWork.ProductRepository.IsProductNameExistsAsync(product.Title)) return false;

            product.ImageUrl = await SetImageUrlAsync(imageFile, webRootPath);

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateResult> UpdateAsync(Product product, IFormFile imageFile, string webRootPath)
        {
            // Get the original product
            var oldProduct = await _unitOfWork.ProductRepository.GetAsync(c => c.Id == product.Id);
            if (oldProduct == null) return UpdateResult.NotFound;

            // Check for duplicate name only if the name has changed
            if (!String.Equals(oldProduct.Title, product.Title, StringComparison.OrdinalIgnoreCase) &&
                await _unitOfWork.ProductRepository.IsProductNameExistsAsync(product.Title, product.Id)) return UpdateResult.DuplicateName;

            // Handle image processing if a new image was uploaded
            if (imageFile != null)
                 await HandleImageUpdate(oldProduct, product, imageFile, webRootPath);
            else if (CheckForNoChanges(oldProduct, product) == UpdateResult.NoChanges)
                return UpdateResult.NoChanges;


            // Update product properties
            UpdateProductProperties(oldProduct, product);


            //save changes
            _unitOfWork.ProductRepository.Update(oldProduct);
            await _unitOfWork.SaveChangesAsync();

            return UpdateResult.Updated;
        }

        public async Task DeleteAsync(int id, string webRootPath)
        {
            var product = await GetProductAsync(c => c.Id == id);

            if (product == null) return;

            if (!String.IsNullOrEmpty(product.ImageUrl))
            {
                string imgPath = Path.Combine(webRootPath, product.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(imgPath))
                    File.Delete(imgPath);
            }

            await _unitOfWork.ProductRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        #region Helpers
        private async Task HandleImageUpdate(Product oldProduct, Product product, IFormFile imageFile, string webRootPath)
        {
            // Delete old image if it exists
            if (!String.IsNullOrEmpty(oldProduct.ImageUrl))
            {
                var oldImagePath = Path.Combine(webRootPath, oldProduct.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Set the new image URL
            product.ImageUrl = await SetImageUrlAsync(imageFile, webRootPath);
        }

        private void UpdateProductProperties(Product oldProduct, Product product)
        {
            oldProduct.Title = product.Title;
            oldProduct.Description = product.Description;
            oldProduct.Author = product.Author;
            oldProduct.ISBN = product.ISBN;
            oldProduct.ListPrice = product.ListPrice;
            oldProduct.Price = product.Price;
            oldProduct.Price50 = product.Price50;
            oldProduct.Price100 = product.Price100;
            oldProduct.CategoryId = product.CategoryId;

            // Only update ImageUrl if it has been set (when an image file was uploaded)
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                oldProduct.ImageUrl = product.ImageUrl;
            }
        }

        private static UpdateResult CheckForNoChanges(Product Old, Product New)
        {
            if (String.Equals(Old.Title, New.Title, StringComparison.OrdinalIgnoreCase)
                && Old.Author == New.Author && Old.Description == New.Description &&
                Old.Price == New.Price && Old.ISBN == New.ISBN && Old.ListPrice == New.ListPrice && Old.Price50 == New.Price50
                && Old.Price100 == New.Price100 && Old.CategoryId == New.CategoryId) return UpdateResult.NoChanges;

            return UpdateResult.Updated;
        }

        private  async Task<string> SetImageUrlAsync(IFormFile imageFile, string webRootPath)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate unique file name using GUID
                string fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                // Create the directory path (without the filename)
                string directoryPath = Path.Combine(webRootPath, "Images", "Products");

                // Ensure the directory exists
                Directory.CreateDirectory(directoryPath);

                // Create the full file path
                string filePath = Path.Combine(directoryPath, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Set the image URL in the product
                return $"/Images/Products/{uniqueFileName}";
            }
            return string.Empty;
        }

        #endregion
    }
}
