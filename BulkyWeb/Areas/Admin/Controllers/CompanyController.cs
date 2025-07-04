using Bookstore.Business.IServices;
using Bookstore.Business.Services;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.Models;
using Bookstore.DataAccess.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            var companies = _companyService.GetAllCompaniesForPaginatedList();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                companies = companies.Where(p =>
                    p.Name.ToLower().Contains(searchTerm));
            }

            var paginatedList = await PaginatedList<Company>.CreateAsync(companies, pageIndex, pageSize);

            return View(paginatedList);
        }

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            var result = await _companyService.AddCompanyAsync(company);
            if(!result)
            {
                ModelState.AddModelError("Name","A company with this name already exists.");
                return View(company);
            }
            TempData["success"] = "Company created successfully";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Company = await _companyService.GetCompanyAsync(c => c.Id == id);
            if (Company == null)
            {
                return NotFound();
            }
            return View(Company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }


            var result = await _companyService.UpdateAsync(company);
            switch (result)
            {
                case UpdateResult.Updated:
                TempData["success"] = "Company updated successfully";
                return RedirectToAction(nameof(Index));

                case UpdateResult.NoChanges:
                    TempData["info"] = "No changes detected";
                    return RedirectToAction(nameof(Index));

                case UpdateResult.DuplicateName:
                    ModelState.AddModelError("Name", "A company with this name already exists.");
                    return View(company);

                case UpdateResult.NotFound:
                    return NotFound();
            }
            return View(company);

        }

        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _companyService.GetCompanyAsync(c => c.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            await _companyService.DeleteAsync(id);

            TempData["success"] = "Company deleted successfully";
            return RedirectToAction(nameof(Index));
        }


        #region Ajax Call
        [HttpGet]
        public async Task<IActionResult> GetCompanies(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            if (pageIndex < 1 || pageSize < 1) return BadRequest("Invalid page index or page size.");

            var companies = _companyService.GetAllCompaniesForPaginatedList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                companies = companies.Where(p =>
                    p.Name.ToLower().Contains(searchTerm));
            }


            var paginatedList = await PaginatedList<Company>.CreateAsync(companies, pageIndex, pageSize);


            if (paginatedList.TotalPages > 0 && pageIndex > paginatedList.TotalPages) return NotFound("Page not found.");


            return PartialView("_CompanyList", paginatedList);
        }

        #endregion
    }
}
