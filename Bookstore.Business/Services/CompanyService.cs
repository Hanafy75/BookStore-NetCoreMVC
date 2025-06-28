using Bookstore.Business.IServices;
using Bookstore.Common.Enums;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Bookstore.Business.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _unitOfWork.CompanyRepository.GetAll().ToListAsync();
        }
        public IQueryable<Company> GetAllCompaniesForPaginatedList()
        {
            return _unitOfWork.CompanyRepository.GetAll();
        }

        public async Task<Company?> GetCompanyAsync(Expression<Func<Company, bool>> predicate)
        {
            return await _unitOfWork.CompanyRepository.GetAsync(predicate);
        }

        public async Task<bool> AddCompanyAsync(Company company)
        {
            //company with the same name exist
            if (await _unitOfWork.CompanyRepository.IsCompanyNameExistsAsync(company.Name)) return false;

            await _unitOfWork.CompanyRepository.AddAsync(company);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateResult> UpdateAsync(Company company)
        {
            var oldCompany = await _unitOfWork.CompanyRepository.GetAsync(c=> c.Id==company.Id);
            if (oldCompany == null) return UpdateResult.NotFound;

            // Check for duplicate name only if the name has changed
            if (!String.Equals(oldCompany.Name, company.Name, StringComparison.OrdinalIgnoreCase) &&
                await _unitOfWork.CompanyRepository.IsCompanyNameExistsAsync(company.Name, company.Id)) return UpdateResult.DuplicateName;

            //check fo no changes
            if(String.Equals(oldCompany.Name,company.Name, StringComparison.OrdinalIgnoreCase)) return UpdateResult.NoChanges;


            oldCompany.Name = company.Name;

            _unitOfWork.CompanyRepository.Update(oldCompany);
            await _unitOfWork.SaveChangesAsync();

            return UpdateResult.Updated;
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.CompanyRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
