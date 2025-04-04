using Coursework.Interfaces.Database;
using Coursework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

public class ProgrammingLanguagesController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var languages = await uow.Languages.GetAllAsync();
        
        ViewBag.Languages = languages;
        
        return View();
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    public async Task<IActionResult> Update(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var language = await uow.Languages.GetAsync(id);
        if (language is null)
            return NotFound();
        
        return View(language);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(ProgrammingLanguage language, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(language);
        
        await using var uow = await uowFactory.CreateAsync(ct);
        var id = await uow.Languages.AddAsync(language);
        await uow.CommitAsync(ct);
        
        logger.LogInformation(
            "Добавлен язык программирования. Id:{Id}, Название:{Name}, Описание:{Description}",
            id, language.Name, language.Description);
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(ProgrammingLanguage language, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(language);
        
        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Languages.UpdateAsync(language);
        await uow.CommitAsync(ct);
        
        logger.LogInformation(
            "Обновлен язык программирования. Id:{Id}, Название:{LanguageName}, Описание:{LanguageDescription}", 
            language.Id, language.Name, language.Description);
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Languages.DeleteAsync(id);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Удален язык программирования. Id:{Id}", id);
        
        return RedirectToAction("Index");
    }
}