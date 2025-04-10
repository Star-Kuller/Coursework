using Coursework.Extensions;
using Coursework.Interfaces.Database;
using Coursework.Models;
using Coursework.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Coursework.Controllers;

[Authorize(Roles = "Администратор")]
public class FrameworksController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var frameworks = await uow.Frameworks.GetAllWithLanguageAsync();
        
        ViewBag.Frameworks = frameworks;
        
        return View();
    }
    
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var languages = await uow.Languages.GetAllAsync();
        
        ViewBag.Languages = languages;

        return View();
    }
    
    public async Task<IActionResult> Update(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var framework = await uow.Frameworks.GetAsync(id);
        if (framework is null)
            return NotFound();
        
        var languages = await uow.Languages.GetAllAsync();
        ViewBag.Languages = languages;
        
        return View(framework);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Framework framework, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        
        if (!ModelState.IsValid)
        {
            var languages = await uow.Languages.GetAllAsync();
            ViewBag.Languages = languages;
            return View(framework);
        }
        
        var id = await uow.Frameworks.AddAsync(framework);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Добавлен фреймворк. Id:{Id}, Название:{LanguageName}, Описание:{LanguageDescription}, Автор: {AuthorId}",
            id, framework.Name, framework.Description, User.GetId());
            
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(Framework framework, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(framework);
        
        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Frameworks.UpdateAsync(framework);
        await uow.CommitAsync(ct);
        
        logger.LogInformation(
            "Обновлен фреймворк. Id:{Id}, Название:{Name}, Описание:{Description}, Id языка программирования:{LanguageId}", 
            framework.Id, framework.Name, framework.Description, framework.LanguageId);
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Frameworks.DeleteAsync(id);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Удален фреймворк. Id:{Id}", id);
        
        return RedirectToAction("Index");
    }
}