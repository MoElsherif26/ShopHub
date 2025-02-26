using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopHub.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Infrastructure.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Description).IsRequired();
            builder.Property(p => p.NewPrice).HasColumnType("decimal(18.2)");
            builder.Property(p => p.OldPrice).HasColumnType("decimal(18.2)");

            // seed data
            builder.HasData(new Product { Id = 1, Name = "TestProduct", Description = "TestProductDescription", CategoryId = 1, NewPrice = 120 });


        }
    }
}
