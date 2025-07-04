using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.DataAccess.EntityConfigurations
{
    internal class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasOne(sc => sc.Product)
                .WithMany()
                .HasForeignKey(sc => sc.ProductId)
                .IsRequired();

            builder.HasOne(sc => sc.ApplicationUser)
                .WithMany()
                .HasForeignKey(fk => fk.ApplicationUserId)
                .IsRequired(); 
        }
    }
}
