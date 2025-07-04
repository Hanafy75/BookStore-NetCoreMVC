using Bookstore.Business.IServices;
using Bookstore.Business.Services;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class UserController : Controller
    {
        private readonly IApplicationUserService _userService;

        public UserController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            var paginatedList = await _userService.GetPaginatedUsersWithRolesAsync(pageIndex, pageSize, searchTerm);
            return View(paginatedList);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var userViewModel = await _userService.GetUSerViewModel(id);
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            await _userService.UpdateUserRole(userViewModel);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUnLock(string id)
        {
            await _userService.LockOrUnlockUserAsync(id);
            TempData["success"] = "Operation Done Successfully";
            return RedirectToAction(nameof(Index));
        }


        #region Ajax Call
        [HttpGet]
        public async Task<IActionResult> GetUsers(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            if (pageIndex < 1 || pageSize < 1)
                return BadRequest("Invalid page index or page size.");

            var paginatedList = await _userService.GetPaginatedUsersWithRolesAsync(pageIndex, pageSize, searchTerm);

            if (paginatedList.TotalPages > 0 && pageIndex > paginatedList.TotalPages)
                return NotFound("Page not found.");

            return PartialView("_UserList", paginatedList);
        }

        #endregion
    }
}
