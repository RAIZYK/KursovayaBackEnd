using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SmartphoneStore.Api.Controllers;
[ApiController,Route("api/uploads"),Authorize(Roles="Admin")]
public class UploadsController(IWebHostEnvironment env) : ControllerBase {
    [HttpPost]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> Upload(IFormFile file) {
        if(file.Length==0)return BadRequest(new{message="Файл порожній."});
        var allowed=new[]{"image/jpeg","image/png","image/webp"};if(!allowed.Contains(file.ContentType))return BadRequest(new{message="Дозволені лише JPG, PNG або WEBP."});
        var folder=Path.Combine(env.WebRootPath??Path.Combine(env.ContentRootPath,"wwwroot"),"uploads");Directory.CreateDirectory(folder);var ext=Path.GetExtension(file.FileName).ToLowerInvariant();var name=$"{Guid.NewGuid():N}{ext}";var path=Path.Combine(folder,name);
        await using(var stream=System.IO.File.Create(path))await file.CopyToAsync(stream);
        return Ok(new{url=$"/uploads/{name}"});
    }
}