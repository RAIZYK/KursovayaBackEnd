using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartphoneStore.Api.Data;
using SmartphoneStore.Api.DTOs;
using SmartphoneStore.Api.Models;
namespace SmartphoneStore.Api.Controllers;
[ApiController]
public class ReviewsController(StoreDbContext db) : ControllerBase {
    [HttpGet("api/products/{productId:int}/reviews")]
    public async Task<IActionResult> Get(int productId) => Ok(await db.Reviews.AsNoTracking().Where(r=>r.ProductId==productId).OrderByDescending(r=>r.CreatedAt).Select(r=>new {r.Id,r.ProductId,r.UserId,UserName=r.User!.FullName,r.Rating,r.Comment,r.CreatedAt}).ToListAsync());
    [Authorize,HttpPost("api/products/{productId:int}/reviews")]
    public async Task<IActionResult> Create(int productId,ReviewRequest request) { if(!await db.Products.AnyAsync(p=>p.Id==productId))return NotFound(); var uid=GetUserId(); var review=new Review{ProductId=productId,UserId=uid,Rating=request.Rating,Comment=request.Comment.Trim()};db.Reviews.Add(review);await db.SaveChangesAsync();return Ok(new {review.Id}); }
    [Authorize,HttpPut("api/reviews/{id:int}")]
    public async Task<IActionResult> Update(int id,ReviewRequest request) { var r=await db.Reviews.FindAsync(id);if(r is null)return NotFound();if(r.UserId!=GetUserId()&&!User.IsInRole("Admin"))return Forbid();r.Rating=request.Rating;r.Comment=request.Comment.Trim();await db.SaveChangesAsync();return NoContent(); }
    [Authorize,HttpDelete("api/reviews/{id:int}")]
    public async Task<IActionResult> Delete(int id) { var r=await db.Reviews.FindAsync(id);if(r is null)return NotFound();if(r.UserId!=GetUserId()&&!User.IsInRole("Admin"))return Forbid();db.Reviews.Remove(r);await db.SaveChangesAsync();return NoContent(); }
    private int GetUserId()=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}