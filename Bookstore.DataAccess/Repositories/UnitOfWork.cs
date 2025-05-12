using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
namespace Bookstore.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository CategoryRepository { get; private set; }

        public IGenericRepository<Product> ProductRepository {  get; private set; }

        public UnitOfWork(AppDbContext context,ICategoryRepository categoryRepository, IGenericRepository<Product> productRepository)
        {
            _context = context;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
