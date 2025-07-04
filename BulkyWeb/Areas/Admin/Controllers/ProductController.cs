using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            this._webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            var products = _productService.GetAllProductsIncludeCategoryName();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                products = products.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Author.ToLower().Contains(searchTerm) ||
                    p.ISBN.ToLower().Contains(searchTerm));
            }

            var paginatedList = await PaginatedList<ProductIndexViewModel>.CreateAsync(products, pageIndex, pageSize);
            return View(paginatedList);
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProductCreateViewModel viewModel = new();
            viewModel.CategoryList = await _categoryService.GetSelectListCategories();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel PviewModel)
        {
            if (!ModelState.IsValid)
            {
                PviewModel.CategoryList = await _categoryService.GetSelectListCategories();
                return View(PviewModel);
            }

            var result = await _productService.AddProductAsync(PviewModel.product, PviewModel.ImageFile, _webHostEnvironment.WebRootPath);
            if (!result)
            {
                ModelState.AddModelError("Title", "A product with this Title already exists.");
                PviewModel.CategoryList = await _categoryService.GetSelectListCategories();// This ensures the dropdown has all category options again so the user can re-submit the form properly.
                                                                                           // => if i didn't do this the list will be empty coz the categoryId is the onlything is posted not the whole list.
                return View(PviewModel);
            }
            TempData["success"] = "product created successfully";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Product? product = await _productService.GetProductAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ProductEditViewModel viewModel = new()
            {
                product = product,
                CategoryList = await _categoryService.GetSelectListCategories()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                viewModel.CategoryList = await _categoryService.GetSelectListCategories();
                return View(viewModel);
            }


            var result = await _productService.UpdateAsync(viewModel.product, viewModel.ImageFile, _webHostEnvironment.WebRootPath);
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
                    return View(viewModel);

                case UpdateResult.NotFound:
                    return NotFound();
            }
            return View(viewModel);

        }

        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id, _webHostEnvironment.WebRootPath);

            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Ajax Call
        [HttpGet]
        public async Task<IActionResult> GetProducts(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            if (pageIndex < 1 || pageSize < 1) return BadRequest("Invalid page index or page size.");

            var products = _productService.GetAllProductsIncludeCategoryName();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                products = products.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Author.ToLower().Contains(searchTerm) ||
                    p.ISBN.ToLower().Contains(searchTerm));
            }


            var paginatedList = await PaginatedList<ProductIndexViewModel>.CreateAsync(products, pageIndex, pageSize);


            if (paginatedList.TotalPages > 0 && pageIndex > paginatedList.TotalPages) return NotFound("Page not found.");


            return PartialView("_ProductList", paginatedList);
        }

        #endregion
    }
}
