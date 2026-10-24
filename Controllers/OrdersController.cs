using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartphoneStore.Api.Data;
using SmartphoneStore.Api.DTOs;
using SmartphoneStore.Api.Models;
namespace SmartphoneStore.Api.Controllers;
[ApiController,Route("api/orders"),Authorize]
public class OrdersController(StoreDbContext db) : ControllerBase {
    [HttpPost]
    public async Task<IActionResult> Create(OrderRequest request) {
        if(request.Items.Select(i=>i.ProductId).Distinct().Count()!=request.Items.Count)return BadRequest(new {message="Один товар не можна дублювати в одному запиті."});
        var ids=request.Items.Select(i=>i.ProductId).ToList(); var products=await db.Products.Where(p=>ids.Contains(p.Id)).ToListAsync(); if(products.Count!=ids.Count)return BadRequest(new {message="Один або декілька товарів не знайдені."});
        foreach(var line in request.Items){var p=products.Single(x=>x.Id==line.ProductId);if(p.Stock<line.Quantity)return Conflict(new {message=$"Недостатньо залишку товару: {p.Name}."});}
        var order=new Order{UserId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),Status="New"};
        foreach(var line in request.Items){var p=products.Single(x=>x.Id==line.ProductId);p.Stock-=line.Quantity;order.Items.Add(new OrderItem{ProductId=p.Id,ProductName=p.Name,UnitPrice=p.Price,Quantity=line.Quantity});order.Total+=p.Price*line.Quantity;}
        db.Orders.Add(order);await db.SaveChangesAsync();return Created($"/api/orders/{order.Id}",new {order.Id,order.Total,order.Status});
    }
    [HttpGet("my")]
    public async Task<IActionResult> Mine(){var uid=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);return Ok(await db.Orders.AsNoTracking().Where(o=>o.UserId==uid).Include(o=>o.Items).OrderByDescending(o=>o.CreatedAt).Select(o=>new{o.Id,o.Status,o.Total,o.CreatedAt,Items=o.Items.Select(i=>new{i.ProductName,i.UnitPrice,i.Quantity})}).ToListAsync());}
    [Authorize(Roles="Admin"),HttpGet]
    public async Task<IActionResult> All()=>Ok(await db.Orders.AsNoTracking().Include(o=>o.User).Include(o=>o.Items).OrderByDescending(o=>o.CreatedAt).Select(o=>new{o.Id,o.UserId,Customer=o.User!.FullName,Email=o.User.Email,o.Status,o.Total,o.CreatedAt,Items=o.Items.Select(i=>new{i.ProductName,i.UnitPrice,i.Quantity})}).ToListAsync());
    [Authorize(Roles="Admin"),HttpPatch("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id,UpdateOrderStatusRequest request){var allowed=new[]{"New","Processing","Shipped","Completed","Cancelled"};if(!allowed.Contains(request.Status))return BadRequest(new{message="Невідомий статус."});var order=await db.Orders.FindAsync(id);if(order is null)return NotFound();order.Status=request.Status;await db.SaveChangesAsync();return NoContent();}
}