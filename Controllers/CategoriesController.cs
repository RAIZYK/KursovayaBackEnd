using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartphoneStore.Api.Data;
namespace SmartphoneStore.Api.Controllers;
[ApiController, Route("api/categories")]
public class CategoriesController(StoreDbContext db) : ControllerBase {
    [HttpGet] public async Task<IActionResult> Get() => Ok(await db.Categories.AsNoTracking().OrderBy(c=>c.Name).Select(c=>new { c.Id,c.Name }).ToListAsync());
}