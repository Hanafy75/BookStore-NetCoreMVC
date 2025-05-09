using Bookstore.Business.IServices;
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
            return View();
        }
    }
}
