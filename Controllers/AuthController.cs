using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartphoneStore.Api.Data;
using SmartphoneStore.Api.DTOs;
using SmartphoneStore.Api.Models;
using SmartphoneStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
namespace SmartphoneStore.Api.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(StoreDbContext db, TokenService tokens) : ControllerBase {
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request) {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email)) return Conflict(new { message = "Користувач із таким email уже існує." });
        var user = new AppUser { FullName=request.FullName.Trim(), Email=email, Role="User" };
        user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, request.Password);
        db.Users.Add(user); await db.SaveChangesAsync();
        return Ok(new AuthResponse(tokens.Create(user), new UserDto(user.Id,user.FullName,user.Email,user.Role)));
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request) {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == request.Email.Trim().ToLowerInvariant());
        if (user is null || new PasswordHasher<AppUser>().VerifyHashedPassword(user,user.PasswordHash,request.Password) == PasswordVerificationResult.Failed) return Unauthorized(new { message="Невірний email або пароль." });
        return Ok(new AuthResponse(tokens.Create(user), new UserDto(user.Id,user.FullName,user.Email,user.Role)));
    }
    [Authorize, HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me() {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) return Unauthorized();
        var user = await db.Users.FindAsync(id); return user is null ? Unauthorized() : Ok(new UserDto(user.Id,user.FullName,user.Email,user.Role));
    }
}