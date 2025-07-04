using Bookstore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.DataAccess.EntityConfigurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.ApplicationUser)
                .WithMany()
                .HasForeignKey(o => o.ApplicationUserId)
                .IsRequired();
        }
    }
}

