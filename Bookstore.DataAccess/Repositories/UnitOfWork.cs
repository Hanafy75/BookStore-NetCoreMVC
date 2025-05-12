using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
namespace Bookstore.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository CategoryRepository { get; private set; }
        public UnitOfWork(AppDbContext context,ICategoryRepository categoryRepository)
        {
            _context = context;
            CategoryRepository = categoryRepository;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
