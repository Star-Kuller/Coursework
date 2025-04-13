using Coursework.Extensions;
using Coursework.Interfaces.Database;
using Coursework.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

[Authorize(Policy = "UserAccess")]
public class UserController(IUnitOfWorkFactory unitOfWorkFactory, ILogger<UserController> logger) : Controller
{
    public async Task<IActionResult> View(long id, CancellationToken ct)
    {
        await using var unitOfWork = await unitOfWorkFactory.CreateAsync(ct);
        var user = await unitOfWork.Users.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
    
    public async Task<IActionResult> Update(long id, CancellationToken ct)
    {
        var currentUserId = User.GetId();
        var isAdmin = User.IsInRole("Администратор");
        
        if (currentUserId != id && !isAdmin)
        {
            return Forbid();
        }
        
        await using var unitOfWork = await unitOfWorkFactory.CreateAsync(ct);
        var user = await unitOfWork.Users.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(User model, CancellationToken ct)
    {
        var currentUserId = User.GetId();
        var isAdmin = User.IsInRole("Администратор");
        
        if (currentUserId != model.Id && !isAdmin)
        {
            return Forbid();
        }
        
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            ModelState.AddModelError("Name", "Имя пользователя не может быть пустым");
            return View("Update", model);
        }
        
        await using var unitOfWork = await unitOfWorkFactory.CreateAsync(ct);
        var user = await unitOfWork.Users.GetAsync(model.Id);

        if (user == null)
        {
            return NotFound();
        }
        
        user.Name = model.Name;
        user.About = model.About ?? string.Empty;
        
        if (isAdmin)
        {
            user.RoleId = model.RoleId;
            user.Score = model.Score;
        }
        
        await unitOfWork.Users.UpdateAsync(user);
        await unitOfWork.CommitAsync(ct);
        
        if(!isAdmin) 
            logger.LogInformation("Пользователь {UserId} обновил профиль", model.Id);
        else
            logger.LogInformation("Администратор {AdminId} изменил пользователя {UserId}", 
                currentUserId, model.Id);
        
        return RedirectToAction("View", new { model.Id });
    }
}