using Coursework.Interfaces.Database;
using Coursework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

public class FrameworksController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var languages = await uow.Languages.GetAllAsync();
        
        ViewBag.Languages = languages;

        return View();
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
        
        logger.LogInformation("Добавлен фреймворк. Id:{Id}, Название:{LanguageName}, Описание:{LanguageDescription}", id, framework.Name, framework.Description);
            
        return RedirectToAction("Index", "Home");
    }
}