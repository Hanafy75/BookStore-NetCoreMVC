using Bookstore.Business.IServices;
using Bookstore.Business.Services;
using Bookstore.Common.SD;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;

        public OrderController(IOrderService orderService, IOrderDetailService orderDetailService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            IQueryable<Order> orders;

            if (User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Employee))
                orders =  _orderService.GetAllOrdersForPaginatedListAsync();
            else          
                orders =  _orderService.GetAllOrdersForPaginatedListAsync(o => o.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                orders = orders.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.PhoneNumber.ToLower().Contains(searchTerm) ||
                    p.OrderStatus.ToLower().Contains(searchTerm) ||
                    (p.ApplicationUser != null && p.ApplicationUser.Email.ToLower().Contains(searchTerm)));
            }


            var paginatedList = await PaginatedList<Order>.CreateAsync(orders, pageIndex, pageSize);

            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            OrderViewModel orderViewModel = new()
            {
                Order = await _orderService.GetOrderIncludeUserAsync(o => o.Id == id),
                OrderDetails = await _orderDetailService.GetAllOrderDetailsIncludeProductAsync(OD => OD.OrderId == id)
            };
            return View(orderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
        public async Task<IActionResult> UpdateOrderDetail(OrderViewModel orderViewModel)
        {
            Order? order = await _orderService.GetOrderAsync(o => o.Id == orderViewModel.Order.Id);


            await _orderService.UpdateAsync(order, orderViewModel);

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Details), new { id = order.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
        public async Task<IActionResult> StartProcessing(OrderViewModel orderViewModel)
        {
            await _orderService.UpdateStatus(orderViewModel.Order.Id, OrderStatus.InProcess);
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { Id = orderViewModel.Order.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
        public async Task<IActionResult> ShipOrder(OrderViewModel orderViewModel)
        {
            Order? order = await _orderService.GetOrderAsync(o => o.Id == orderViewModel.Order.Id);

            await _orderService.UpdateForShipOrderAsync(order, orderViewModel);



            TempData["Success"] = "Order Shiped Successfully.";
            return RedirectToAction(nameof(Details), new { Id = orderViewModel.Order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
        public async Task<IActionResult> CancelOrder(OrderViewModel orderViewModel)
        {

            var order = await _orderService.GetOrderAsync(u => u.Id == orderViewModel.Order.Id);

            if (order.PaymentStatus == PaymentStatus.Approved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                await _orderService.UpdateStatus(order.Id, OrderStatus.Cancelled, OrderStatus.Refunded);
            }
            else
            {
               await  _orderService.UpdateStatus(order.Id, OrderStatus.Cancelled, OrderStatus.Cancelled);
            }
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { Id = orderViewModel.Order.Id });

        }


        [ActionName("Details")]
        [HttpPost]
        public async Task<IActionResult> Details_PAY_NOW(OrderViewModel orderViewModel)
        {
            orderViewModel.Order = await _orderService.GetOrderIncludeUserAsync(u => u.Id == orderViewModel.Order.Id);
            orderViewModel.OrderDetails = await _orderDetailService.GetAllOrderDetailsIncludeProductAsync(u => u.OrderId == orderViewModel.Order.Id);

            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?Id={orderViewModel.Order.Id}",
                CancelUrl = domain + $"admin/order/details?Id={orderViewModel.Order.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in orderViewModel.OrderDetails)
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
            await _orderService.UpdateStripePaymentID(orderViewModel.Order.Id, session.Id, session.PaymentIntentId);
            Response.Headers["Location"] = session.Url?.ToString();
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> PaymentConfirmation(int Id)
        {

            Order? order = await _orderService.GetOrderAsync(u => u.Id == Id);
            if (order.PaymentStatus == PaymentStatus.DelayedPayment)
            {
                //this is an order by company

                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _orderService.UpdateStripePaymentID(Id, session.Id, session.PaymentIntentId);
                    await _orderService.UpdateStatus(Id, order.OrderStatus, PaymentStatus.Approved);
                }


            }


            return View(Id);
        }


        #region Ajax Call
        [HttpGet]
        public async Task<IActionResult> GetOrders(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            if (pageIndex < 1 || pageSize < 1) return BadRequest("Invalid page index or page size.");

            IQueryable<Order> orders;

            if (User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Employee))
                orders = _orderService.GetAllOrdersForPaginatedListAsync();
            else
                orders = _orderService.GetAllOrdersForPaginatedListAsync(o => o.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                orders = orders.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.PhoneNumber.ToLower().Contains(searchTerm) ||
                    p.OrderStatus.ToLower().Contains(searchTerm) ||
                   (p.ApplicationUser != null && p.ApplicationUser.Email.ToLower().Contains(searchTerm)));
            }


            var paginatedList = await PaginatedList<Order>.CreateAsync(orders, pageIndex, pageSize);


            if (paginatedList.TotalPages > 0 && pageIndex > paginatedList.TotalPages) return NotFound("Page not found.");


            return PartialView("_OrderList", paginatedList);
        }

        #endregion
    }
}
