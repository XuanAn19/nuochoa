using CK_ASP_NET_CORE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CK_ASP_NET_CORE.Repository
{
    public class DataContext : IdentityDbContext<AppUseModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<OrderModel> OrderModels { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
