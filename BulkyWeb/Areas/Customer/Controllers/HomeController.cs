using System.Diagnostics;
using System.Security.Claims;
using Bookstore.Business.IServices;
using Bookstore.Common.SD;
using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _productService = productService;
            this._shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null  )
            {
                //if User logged in we set the session so wew can retrive it in the _layout
                int shoppingCartCount = (await _shoppingCartService.GetAllShoppingForUserCartsAsync(sc => sc.ApplicationUserId == userId)).Count();

                HttpContext.Session.SetInt32(SessionKeys.SessionCart, shoppingCartCount);
            }

            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int Id)
        {
            if (Id == 0) return NotFound();

            ShoppingCart cart = new()
            {
                Product = await _productService.GetProductWithCategoryAsync(p => p.Id == Id),
                Count = 1,
                ProductId = Id
            };
            return View(cart);
        }

        [Authorize]
        public async Task<IActionResult> Details([Bind("ProductId,Count")] ShoppingCart cart)
        {
            //get userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //populate UserId
            cart.ApplicationUserId = userId!;

             await _shoppingCartService.AddShoppingCartAsync(cart, userId!);
            // get count of shpping cart for logged in user
            int shoppingCartCount = (await _shoppingCartService.GetAllShoppingForUserCartsAsync(sc=> sc.ApplicationUserId == userId)).Count();
            // set it in session
            HttpContext.Session.SetInt32(SessionKeys.SessionCart, shoppingCartCount);
            TempData["success"] = "Cart Updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
