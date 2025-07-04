using Bookstore.Business.IServices;
using Bookstore.Common.SD;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;

        public CartController(IShoppingCartService shoppingCartService, IApplicationUserService applicationUserService, IOrderService orderService, IOrderDetailService orderDetailService)
        {
            _shoppingCartService = shoppingCartService;
            _applicationUserService = applicationUserService;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel cartViewModel = new()
            {
                ShoppingCartList = await _shoppingCartService.GetAllShoppingCartsIncludeProductsAsync(sc => sc.ApplicationUserId == userId),
                Order = new()
            };

            _shoppingCartService.CalcOrderTotal(cartViewModel);
            return View(cartViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel cartViewModel = new()
            {
                ShoppingCartList = await _shoppingCartService.GetAllShoppingCartsIncludeProductsAsync(sc => sc.ApplicationUserId == userId),
                Order = new()
            };

            cartViewModel.Order.ApplicationUser = await _applicationUserService.GetUserAsync(u => u.Id == userId);

            cartViewModel.Order.Name = cartViewModel.Order.ApplicationUser.Name;
            cartViewModel.Order.PhoneNumber = cartViewModel.Order.ApplicationUser.PhoneNumber;
            cartViewModel.Order.StreetAddress = cartViewModel.Order.ApplicationUser.StreetAddress;
            cartViewModel.Order.City = cartViewModel.Order.ApplicationUser.City;
            cartViewModel.Order.State = cartViewModel.Order.ApplicationUser.State;
            cartViewModel.Order.PostalCode = cartViewModel.Order.ApplicationUser.PostalCode;

            _shoppingCartService.CalcOrderTotal(cartViewModel);
            return View(cartViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(ShoppingCartViewModel cartViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            cartViewModel.ShoppingCartList = await _shoppingCartService.GetAllShoppingCartsIncludeProductsAsync(sc => sc.ApplicationUserId == userId);

            cartViewModel.Order.ApplicationUserId = userId;
            cartViewModel.Order.OrderDate = DateTime.Now;
            ApplicationUser user = await _applicationUserService.GetUserAsync(u => u.Id == userId);

            _shoppingCartService.CalcOrderTotal(cartViewModel);


            //check if it's CompanyUser Or CustomerUser
            if (user?.CompanyId.GetValueOrDefault() == 0)
            {
                //if yes so it's a customer user
                cartViewModel.Order.OrderStatus = OrderStatus.Pending;
                cartViewModel.Order.PaymentStatus = PaymentStatus.Pending;
            }
            else
            {
                //so it's a Company User
                cartViewModel.Order.OrderStatus = OrderStatus.Approved;
                cartViewModel.Order.PaymentStatus = PaymentStatus.DelayedPayment;
            }

            //add Order
            await _orderService.AddOrderAsync(cartViewModel.Order);

            foreach (var cart in cartViewModel.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = cartViewModel.Order.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                await _orderDetailService.AddOrderDetailAsync(orderDetail);
            }

            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                //it's a cutomer accont so we need to capture payment
                //stripe logic
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={cartViewModel.Order.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in cartViewModel.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);
                await _orderService.UpdateStripePaymentID(cartViewModel.Order.Id, session.Id, session.PaymentIntentId);
                Response.Headers["Location"] = session.Url?.ToString();
                return new StatusCodeResult(303);

            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = cartViewModel.Order.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            Order order = await _orderService.GetOrderAsync(o=> o.Id== id);
            order.ApplicationUser = await _applicationUserService.GetUserAsync(u => u.Id == order.ApplicationUserId);
            if (order.PaymentStatus != PaymentStatus.DelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                   await _orderService.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                   await _orderService.UpdateStatus(id, OrderStatus.Approved, PaymentStatus.Approved);
                }
                HttpContext.Session.Clear();
            }

            IEnumerable<ShoppingCart> shoppingCarts = await _shoppingCartService
               .GetAllShoppingCartsIncludeProductsAsync(u => u.ApplicationUserId == order.ApplicationUserId);
            await _shoppingCartService.RemoveRange(shoppingCarts);
            
            return View(id);
        }

        public async Task<IActionResult> Plus(int Id)
        {
            var cartFromDb = await _shoppingCartService.GetShoppingCartAsync(sc => sc.Id == Id);
            cartFromDb.Count += 1;
            await _shoppingCartService.UpdateAsync(cartFromDb);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int Id)
        {
            var cartFromDb = await _shoppingCartService.GetShoppingCartAsync(sc => sc.Id == Id);
            if (cartFromDb.Count <= 1)
            {
                await _shoppingCartService.DeleteAsync(Id);
                //reset the seession data
                int shoppingCartCount = (await _shoppingCartService.GetAllShoppingForUserCartsAsync(sc => sc.ApplicationUserId == cartFromDb.ApplicationUserId)).Count();
                HttpContext.Session.SetInt32(SessionKeys.SessionCart, shoppingCartCount);

            }
            else
            {
                cartFromDb.Count -= 1;
                await _shoppingCartService.UpdateAsync(cartFromDb);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int Id)
        {
            await _shoppingCartService.DeleteAsync(Id);

            //reset the seession data
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int shoppingCartCount = (await _shoppingCartService.GetAllShoppingForUserCartsAsync(sc => sc.ApplicationUserId == userId)).Count();
            HttpContext.Session.SetInt32(SessionKeys.SessionCart, shoppingCartCount);

            return RedirectToAction(nameof(Index));
        }
    }
}
