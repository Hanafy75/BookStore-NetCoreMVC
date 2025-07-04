using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.IRepositories;
using Bookstore.DataAccess.Models;
namespace Bookstore.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository CategoryRepository { get; private set; }

        public IProductRepository ProductRepository {  get; private set; }

        public ICompanyRepository CompanyRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; }

        public IApplicationUserRepository ApplicationUserRepository { get; }

        public IOrderRepository OrderRepository  { get; }

        public IOrderDetailRepository OrderDetailRepository { get; }


        public UnitOfWork(AppDbContext context, ICategoryRepository categoryRepository, IProductRepository productRepository, ICompanyRepository companyRepository, IShoppingCartRepository shoppingCartRepository, IApplicationUserRepository applicationUserRepository, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _context = context;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            CompanyRepository = companyRepository;
            ShoppingCartRepository = shoppingCartRepository;
            ApplicationUserRepository = applicationUserRepository;
            OrderRepository = orderRepository;
            OrderDetailRepository = orderDetailRepository;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
