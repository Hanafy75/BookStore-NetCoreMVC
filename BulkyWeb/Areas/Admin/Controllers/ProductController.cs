using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var result = await _productService.AddProductAsync(product);
            if (!result)
            {
                ModelState.AddModelError("Title", "A product with this Title already exists.");
                return View(product);
            }
            TempData["success"] = "product created successfully";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }


            var result = await _productService.UpdateAsync(product);
            switch (result)
            {
                case UpdateResult.Updated:
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(Index));

                case UpdateResult.NoChanges:
                    TempData["info"] = "No changes detected";
                    return RedirectToAction(nameof(Index));

                case UpdateResult.DuplicateName:
                    ModelState.AddModelError("Title", "A Product with this Title already exists.");
                    return View(product);

                case UpdateResult.NotFound:
                    return NotFound();
            }
            return View(product);

        }
        
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteAsync(id);

            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
