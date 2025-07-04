using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bookstore.DataAccess.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Start from current directory and navigate to MVC project folder
            var currentDirectory = Directory.GetCurrentDirectory();

            // Traverse to the solution root and then to the MVC project's folder
            var mvcProjectPath = Path.Combine(currentDirectory, "..", "BulkyWeb");
            var fullMvcProjectPath = Path.GetFullPath(mvcProjectPath);

            // Load configuration from the MVC project’s appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(fullMvcProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
