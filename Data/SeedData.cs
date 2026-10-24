using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartphoneStore.Api.Models;
namespace SmartphoneStore.Api.Data;
public static class SeedData {
    public static async Task InitializeAsync(IServiceProvider services) {
        var db = services.GetRequiredService<StoreDbContext>();
        if (!await db.Categories.AnyAsync()) {
            db.Categories.AddRange(new Category { Name="Смартфони" }, new Category { Name="Аксесуари" }, new Category { Name="Планшети" });
            await db.SaveChangesAsync();
        }
        if (!await db.Products.AnyAsync()) {
            var phones = await db.Categories.FirstAsync(c => c.Name == "Смартфони");
            db.Products.AddRange(
                new Product { Name="iPhone 15 128GB", Brand="Apple", Description="Смартфон із яскравим дисплеєм, якісною камерою та швидкою роботою для щоденних задач.", Price=32999, Stock=12, CategoryId=phones.Id, ImageUrl="https://images.unsplash.com/photo-1592750475338-74b7b21085ab?auto=format&fit=crop&w=900&q=80" },
                new Product { Name="Galaxy S24 256GB", Brand="Samsung", Description="Флагманський смартфон із високою продуктивністю та універсальною камерою.", Price=29999, Stock=9, CategoryId=phones.Id, ImageUrl="https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=900&q=80" },
                new Product { Name="Xiaomi 14T 256GB", Brand="Xiaomi", Description="Швидкий смартфон із великим дисплеєм і містким накопичувачем.", Price=18999, Stock=18, CategoryId=phones.Id, ImageUrl="https://images.unsplash.com/photo-1598327105666-5b89351aff97?auto=format&fit=crop&w=900&q=80" },
                new Product { Name="Pixel 8 128GB", Brand="Google", Description="Компактний смартфон із чистим Android та обчислювальною фотографією.", Price=24999, Stock=6, CategoryId=phones.Id, ImageUrl="https://images.unsplash.com/photo-1598327105666-5b89351aff97?auto=format&fit=crop&w=900&q=80" });
            await db.SaveChangesAsync();
        }
        if (!await db.Users.AnyAsync(u => u.Email == "admin@smartphonestore.local")) {
            var hasher = new PasswordHasher<AppUser>(); var admin = new AppUser { FullName="Store Administrator", Email="admin@smartphonestore.local", Role="Admin" };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!"); db.Users.Add(admin); await db.SaveChangesAsync();
        }
    }
}