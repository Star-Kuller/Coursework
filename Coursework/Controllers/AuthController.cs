using Coursework.Interfaces.Database;
using Coursework.Interfaces.Services;
using Coursework.Models.DTOs;
using Coursework.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

public class AuthController(
    IUnitOfWorkFactory uowFactory, 
    IJwtService jwtService, 
    IPasswordService passwordService,
    ILogger<AuthController> logger) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        await using var uow = await uowFactory.CreateAsync(ct);
        var user = await uow.Users.GetByEmailAsync(model.Email);
        
        if (user == null || !passwordService.VerifyPassword(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Неверный email или пароль");
            return View(model);
        }
        
        var token = jwtService.GenerateToken(user.Id, user.Name, user.Role?.Name ?? "Пользователь");
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });
        
        logger.LogInformation("Пользователь {Email} успешно вошел в систему", user.Email);
        
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        await using var uow = await uowFactory.CreateAsync(ct);
        
        var existingUser = await uow.Users.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
            return View(model);
        }
        
        // Получаем роль "Пользователь" (ID = 2)
        const long userRoleId = 2;
        var role = await uow.Roles.GetAsync(userRoleId);
        if (role is null)
            throw new InvalidOperationException("Роль \"Пользователь\" отсутствует или задана корректно");
        
        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = passwordService.HashPassword(model.Password),
            RoleId = role.Id,
            About = "",
            Score = 0
        };
        
        var userId = await uow.Users.AddAsync(user);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Зарегистрирован новый пользователь: {Email}", user.Email);

        var token = jwtService.GenerateToken(userId, user.Name, role.Name);
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });
        
        return RedirectToAction("Index", "Home");
    }
    
    [Authorize]
    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        
        return RedirectToAction("Index", "Home");
    }
}