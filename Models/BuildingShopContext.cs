using Microsoft.EntityFrameworkCore;

namespace BuildingShopCore.Models
{
    public class BuildingShopContext:DbContext
    {
        public BuildingShopContext(DbContextOptions<BuildingShopContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.CategoryId);
        }

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Order> Orders { get; set; }= null!;
        public DbSet<OrderProduct> OrderProducts { get; set; } = null!;
        public DbSet<Product> Products { get; set; }=null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;

    }
}
