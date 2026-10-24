using Microsoft.EntityFrameworkCore;
using SmartphoneStore.Api.Models;
namespace SmartphoneStore.Api.Data;
public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options) {
    public DbSet<AppUser> Users => Set<AppUser>(); public DbSet<Product> Products => Set<Product>(); public DbSet<Category> Categories => Set<Category>(); public DbSet<Review> Reviews => Set<Review>(); public DbSet<Order> Orders => Set<Order>(); public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<AppUser>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Product>().Property(x => x.Price).HasPrecision(12,2);
        b.Entity<Order>().Property(x => x.Total).HasPrecision(12,2);
        b.Entity<OrderItem>().Property(x => x.UnitPrice).HasPrecision(12,2);
        b.Entity<Product>().HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Review>().HasOne(x => x.Product).WithMany(x => x.Reviews).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Review>().HasOne(x => x.User).WithMany(x => x.Reviews).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Order>().HasOne(x => x.User).WithMany(x => x.Orders).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<OrderItem>().HasOne(x => x.Order).WithMany(x => x.Items).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}