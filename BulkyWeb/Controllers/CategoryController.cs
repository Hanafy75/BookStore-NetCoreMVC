using Bookstore.Business.IServices;
using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var Categories = await _categoryService.GetAllCategoriesAsync();
            return View(Categories);
        }

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _categoryService.AddCategoryAsync(category);
            TempData["success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Category = await _categoryService.GetCategoryAsync(c => c.Id == id);
            if (Category == null)
            {
                TempData["error"] = "Category not found";
                return NotFound();
            }
            return View(Category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }


            var result = await _categoryService.UpdateAsync(category);
            if (result)
            {
                TempData["success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("Name", "A category with this name already exists.");
            return View(category);
        }

        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryAsync(c => c.Id == id);
            if (category == null)
            {
                TempData["error"] = "Category not found";
                return NotFound();
            }

            await _categoryService.DeleteAsync(id);

            TempData["success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
