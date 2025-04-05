using Coursework.Interfaces.Database;
using Coursework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

public class ExercisesController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(string search, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var exercises = await uow.Exercises.GetAllAsync(search);
        
        ViewBag.Exercises = exercises;
        ViewBag.Search = search;
        
        return View();
    }
    
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        
        var difficultyLevels = await uow.DifficultyLevels.GetAllAsync();
        var frameworks = await uow.Frameworks.GetAllAsync();

        ViewBag.DifficultyLevels = difficultyLevels;
        ViewBag.Frameworks = frameworks;

        return View();
    }
    
    public async Task<IActionResult> Update(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var exercise = await uow.Exercises.GetAsync(id);
        if (exercise is null)
            return NotFound();
        
        var difficultyLevels = await uow.DifficultyLevels.GetAllAsync();
        var frameworks = await uow.Frameworks.GetAllAsync();

        ViewBag.DifficultyLevels = difficultyLevels;
        ViewBag.Frameworks = frameworks;
        ViewBag.SelectedFrameworks = exercise.Frameworks!;
        
        return View(exercise);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Exercise exercise, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);

        if (!ModelState.IsValid)
        {
            var difficultyLevels = await uow.DifficultyLevels.GetAllAsync();
            var frameworks = await uow.Frameworks.GetAllAsync();

            ViewBag.DifficultyLevels = difficultyLevels;
            ViewBag.Frameworks = frameworks;
            ViewBag.SelectedFrameworks = exercise.Frameworks!;

            return View(exercise);
        }

        var id = await uow.Exercises.AddAsync(exercise);
        await uow.CommitAsync(ct);

        logger.LogInformation(
            "Добавлено упражнение. Id:{Id}, Название:{Name}, Баллы:{Score}", 
            id, exercise.Name, exercise.Score);

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(Exercise exercise, CancellationToken ct)
    {
        ViewBag.SelectedFrameworks = exercise.Frameworks!;
        if (!ModelState.IsValid) return View(exercise);
        
        await using var uow = await uowFactory.CreateAsync(ct);
        var prev = await uow.Exercises.GetAsync(exercise.Id);
        if (prev is null)
            return NotFound();
        
        if (prev.IsPublished)
            exercise.IsPublished = true;
        
        await uow.Exercises.UpdateAsync(exercise);
        await uow.CommitAsync(ct);
        
        logger.LogInformation(
            "Обновлено упражнение. Id:{Id}, Название:{LanguageName}, Баллы:{Score}", 
            exercise.Id, exercise.Name, exercise.Score);
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Exercises.DeleteAsync(id);
        await uow.CommitAsync(ct);
        
        logger.LogInformation("Удалено упражнение. Id:{Id}", id);
        
        return RedirectToAction("Index");
    }
}