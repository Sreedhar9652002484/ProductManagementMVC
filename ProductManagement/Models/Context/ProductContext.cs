using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MVCProject.Models;

namespace MVCProject.Models.Context
{
    public class ProductContext : DbContext
    {
        public ProductContext() { }
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
