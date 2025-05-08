using Microsoft.EntityFrameworkCore;

namespace Bookstore.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    }
}
