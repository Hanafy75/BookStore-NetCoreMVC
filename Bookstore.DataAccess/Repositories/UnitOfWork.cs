using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
namespace Bookstore.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository CategoryRepository { get; private set; }

        public IProductRepository ProductRepository {  get; private set; }

        public ICompanyRepository CompanyRepository { get; private set; }

        public UnitOfWork(AppDbContext context,ICategoryRepository categoryRepository, IProductRepository productRepository, ICompanyRepository companyRepository)
        {
            _context = context;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            CompanyRepository = companyRepository;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
