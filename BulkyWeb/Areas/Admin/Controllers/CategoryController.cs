using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
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

            var result = await _categoryService.AddCategoryAsync(category);
            if(!result)
            {
                ModelState.AddModelError("Name","A category with this name already exists.");
                return View(category);
            }
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
            switch (result)
            {
                case UpdateResult.Updated:
                TempData["success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));

                case UpdateResult.NoChanges:
                    TempData["info"] = "No changes detected";
                    return RedirectToAction(nameof(Index));

                case UpdateResult.DuplicateName:
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(category);

                case UpdateResult.NotFound:
                    return NotFound();
            }
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
                return NotFound();
            }

            await _categoryService.DeleteAsync(id);

            TempData["success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
