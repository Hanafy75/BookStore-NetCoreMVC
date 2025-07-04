using Bookstore.Business.IServices;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Bookstore.Business.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _unitOfWork.ApplicationUserRepository.GetAll().ToListAsync();
        }
        public async Task<PaginatedList<ApplicationUser>> GetPaginatedUsersWithRolesAsync(int pageIndex, int pageSize, string searchTerm = null)
        {
            var usersQuery = _unitOfWork.ApplicationUserRepository.GetAll();

            // Appling search filter at database level
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                usersQuery = usersQuery.Where(p => p.Name.ToLower().Contains(searchTerm)); //append search to query
            }

            // Get paginated users
            var paginatedUsers = await PaginatedList<ApplicationUser>.CreateAsync(usersQuery, pageIndex, pageSize);

            // Then get roles only for the users in the current page
            foreach (var user in paginatedUsers.Items)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            }

            return paginatedUsers;
        }

        public async Task<ApplicationUser?> GetUserAsync(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return await _unitOfWork.ApplicationUserRepository.GetAsync(predicate);
        }

        public async Task<UserViewModel> GetUSerViewModel(string id)
        {
            var companies = await _unitOfWork.CompanyRepository.GetAll().ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            var user = await _unitOfWork.ApplicationUserRepository.GetUserIncludeCompanyAsync(u => u.Id == id);
            user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            UserViewModel userViewModel = new()
            {
                User = user,
                Companies = new SelectList(companies, "Id", "Name"),
                Roles = new SelectList(roles, "Name", "Name", user.Role)
            };

            return userViewModel;
        }

        public async Task UpdateUserRole(UserViewModel userViewModel)
        {
            if (userViewModel?.User == null)
            {
                throw new ArgumentNullException(nameof(userViewModel));
            }

            var user = await _unitOfWork.ApplicationUserRepository.GetAsync(o => o.Id == userViewModel.User.Id);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userViewModel.User.Id} not found.");
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            var oldRole = oldRoles.FirstOrDefault();
            var newRole = userViewModel.User.Role;

            bool roleChanged = newRole != oldRole;
            bool companyChanged = userViewModel.User.CompanyId != user.CompanyId;

            // Handle role changes
            if (roleChanged)
            {
                // Remove old role if it exists
                if (!string.IsNullOrEmpty(oldRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, oldRole);
                }

                // Add new role if it's not null or empty
                if (!string.IsNullOrEmpty(newRole))
                {
                    await _userManager.AddToRoleAsync(user, newRole);
                }
            }

            // Handle company assignment
            if (newRole == Roles.Company && companyChanged)
            {
                user.CompanyId = userViewModel.User.CompanyId;
                await _unitOfWork.ApplicationUserRepository.UpdateAsync(user);
            }
            else if (newRole != Roles.Company && user.CompanyId.HasValue)
            {
                // Clear company assignment if user is no longer in Company role
                user.CompanyId = null;
                await _unitOfWork.ApplicationUserRepository.UpdateAsync(user);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> AddUserAsync(ApplicationUser user)
        {
            await _unitOfWork.ApplicationUserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task LockOrUnlockUserAsync(string id)
        {
            var user = await _unitOfWork.ApplicationUserRepository.GetAsync(u => u.Id == id);

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            await _unitOfWork.ApplicationUserRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(string id)
        {
            await _unitOfWork.ApplicationUserRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
