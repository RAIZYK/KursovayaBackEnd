using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartphoneStore.Api.Data;
using SmartphoneStore.Api.DTOs;
using SmartphoneStore.Api.Models;
namespace SmartphoneStore.Api.Controllers;
[ApiController, Route("api/products")]
public class ProductsController(StoreDbContext db) : ControllerBase {
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page=1,[FromQuery] int pageSize=8,[FromQuery] string? search=null,[FromQuery] int? categoryId=null,[FromQuery] string? sort=null) {
        page=Math.Max(1,page); pageSize=Math.Clamp(pageSize,1,40);
        var query=db.Products.AsNoTracking().Include(p=>p.Category).AsQueryable();
        if(!string.IsNullOrWhiteSpace(search)) { var term=search.Trim(); query=query.Where(p=>p.Name.Contains(term)||p.Brand.Contains(term)||p.Description.Contains(term)); }
        if(categoryId.HasValue) query=query.Where(p=>p.CategoryId==categoryId.Value);
        query=sort switch { "price-asc"=>query.OrderBy(p=>p.Price), "price-desc"=>query.OrderByDescending(p=>p.Price), "name"=>query.OrderBy(p=>p.Name), _=>query.OrderByDescending(p=>p.CreatedAt) };
        var total=await query.CountAsync(); var items=await query.Skip((page-1)*pageSize).Take(pageSize).Select(p=>new { p.Id,p.Name,p.Brand,p.Description,p.Price,p.Stock,p.ImageUrl,p.CategoryId,Category=p.Category!.Name }).ToListAsync();
        return Ok(new { items,total,page,pageSize,totalPages=(int)Math.Ceiling(total/(double)pageSize) });
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) { var p=await db.Products.AsNoTracking().Include(x=>x.Category).Where(x=>x.Id==id).Select(x=>new { x.Id,x.Name,x.Brand,x.Description,x.Price,x.Stock,x.ImageUrl,x.CategoryId,Category=x.Category!.Name }).SingleOrDefaultAsync(); return p is null?NotFound(new {message="Товар не знайдено."}):Ok(p); }
    [Authorize(Roles="Admin"),HttpPost]
    public async Task<IActionResult> Create(ProductRequest request) { if(!await db.Categories.AnyAsync(c=>c.Id==request.CategoryId)) return BadRequest(new {message="Категорію не знайдено."}); var p=new Product { Name=request.Name.Trim(),Brand=request.Brand.Trim(),Description=request.Description,Price=request.Price,Stock=request.Stock,ImageUrl=request.ImageUrl,CategoryId=request.CategoryId }; db.Products.Add(p); await db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById),new {id=p.Id},new {p.Id,p.Name}); }
    [Authorize(Roles="Admin"),HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id,ProductRequest request) { var p=await db.Products.FindAsync(id); if(p is null)return NotFound(); if(!await db.Categories.AnyAsync(c=>c.Id==request.CategoryId))return BadRequest(new {message="Категорію не знайдено."}); p.Name=request.Name.Trim();p.Brand=request.Brand.Trim();p.Description=request.Description;p.Price=request.Price;p.Stock=request.Stock;p.ImageUrl=request.ImageUrl;p.CategoryId=request.CategoryId;await db.SaveChangesAsync();return NoContent(); }
    [Authorize(Roles="Admin"),HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var p=await db.Products.FindAsync(id);if(p is null)return NotFound();db.Products.Remove(p);await db.SaveChangesAsync();return NoContent(); }
}