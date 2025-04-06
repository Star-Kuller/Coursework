using Coursework.Interfaces.Database;
using Coursework.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

public class SolutionController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    public IActionResult Index(long id)
    {
        return View();
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    public async Task<IActionResult> Update(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var solution = await uow.Solutions.GetAsync(id);
        if (solution is null)
            return NotFound();
        
        return View(solution);
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Create(Solution solution, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(solution);
        
        await using var uow = await uowFactory.CreateAsync(ct);
        var id = await uow.Solutions.AddAsync(solution);
        await uow.CommitAsync(ct);
        
        logger.LogInformation(
            "Добавлено решение. Id:{Id}, Id упражнения:{ExerciseId}", 
            solution.Id, solution.ExerciseId);
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(Solution solution, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(solution);
        
        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Solutions.UpdateAsync(solution);
        await uow.CommitAsync(ct);
        
        logger.LogInformation(
            "Обновлено решение. Id:{Id}, Id упражнения:{ExerciseId}", 
            solution.Id, solution.ExerciseId);
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Solutions.DeleteAsync(id);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Удалено решение. Id:{Id}", id);
        
        return RedirectToAction("Index");
    }
}