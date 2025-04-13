using Coursework.Extensions;
using Coursework.Interfaces.Database;
using Coursework.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Coursework.Controllers;

[Authorize(Policy = "UserAccess")]
public class SolutionController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Create(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var exercise = await uow.Exercises.GetAsync(id);
        if (exercise is null)
            return NotFound();

        ViewBag.Exercise = exercise;

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
    
    public async Task<IActionResult> View(long id, CancellationToken ct)
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
        solution.AuthorId = User.GetId();
        await uow.Solutions.AddAsync(solution);
        var exercise = await uow.Exercises.GetAsync(solution.ExerciseId);
        var user = await uow.Users.GetAsync(solution.AuthorId);
        if (user is not null && exercise is not null)
        {
            user.Score += exercise.Score;
            await uow.Users.UpdateAsync(user);
        }
        await uow.CommitAsync(ct);
        
        logger.LogInformation(
            "Добавлено решение. Id:{Id}, Id упражнения:{ExerciseId}", 
            solution.Id, solution.ExerciseId);
        
        return RedirectToAction("View", "Exercises", new { Id = solution.ExerciseId });
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
        
        return RedirectToAction("View", "Exercises", new { Id = solution.ExerciseId });
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var solution = await uow.Solutions.GetAsync(id);
        if (solution is null)
            return NotFound();
        
        await uow.Solutions.DeleteAsync(id);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Удалено решение. Id:{Id}", id);
        
        return RedirectToAction("View", "Exercises", new { Id = solution.ExerciseId });
    }
}