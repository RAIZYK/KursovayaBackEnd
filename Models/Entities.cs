using System.ComponentModel.DataAnnotations;
namespace SmartphoneStore.Api.Models;

public class AppUser {
    public int Id { get; set; }
    [MaxLength(120)] public string FullName { get; set; } = "";
    [MaxLength(180)] public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    [MaxLength(20)] public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Review> Reviews { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}
public class Category {
    public int Id { get; set; }
    [MaxLength(80)] public string Name { get; set; } = "";
    public List<Product> Products { get; set; } = [];
}
public class Product {
    public int Id { get; set; }
    [MaxLength(160)] public string Name { get; set; } = "";
    [MaxLength(100)] public string Brand { get; set; } = "";
    [MaxLength(3000)] public string Description { get; set; } = "";
    [Range(0, 100000000)] public decimal Price { get; set; }
    [Range(0, 100000)] public int Stock { get; set; }
    [MaxLength(500)] public string ImageUrl { get; set; } = "";
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Review> Reviews { get; set; } = [];
}
public class Review {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
    [Range(1, 5)] public int Rating { get; set; }
    [MaxLength(1200)] public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class Order {
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "New";
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = [];
}
public class OrderItem {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    [MaxLength(160)] public string ProductName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}