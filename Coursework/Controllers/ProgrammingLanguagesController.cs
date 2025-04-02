using Coursework.Interfaces.Database;
using Coursework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

public class ProgrammingLanguagesController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(ProgrammingLanguage language, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(language);
        
        logger.LogTrace($"Добавлен язык программирования. Название:{language.Name}, Описание:{language.Description}");
        
        await using var uow = await uowFactory.CreateAsync(ct);
        var id = await uow.Languages.AddAsync(language);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Добавлен язык программирования. Id:{Id}, Название:{LanguageName}, Описание:{LanguageDescription}", id, language.Name, language.Description);
        
        return RedirectToAction("Index", "Home");
    }
}