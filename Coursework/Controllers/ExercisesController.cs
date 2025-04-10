using Coursework.Extensions;
using Coursework.Interfaces.Database;
using Coursework.Models.DTOs;
using Coursework.Models.Entities;
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
        var languages = await uow.Languages.GetAllAsync();
        var frameworks = await uow.Frameworks.GetAllAsync();

        ViewBag.DifficultyLevels = difficultyLevels;
        ViewBag.Languages = languages;
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
        var languages = await uow.Languages.GetAllAsync();
        var frameworks = await uow.Frameworks.GetAllAsync();

        ViewBag.DifficultyLevels = difficultyLevels;
        ViewBag.Languages = languages;
        ViewBag.Frameworks = frameworks;
        
        return View(exercise.Map());
    }

    public async Task<IActionResult> View(long id, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var exercise = await uow.Exercises.GetAsync(id);
        if (exercise is null)
            return NotFound();
        
        return View(exercise.Map());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ExerciseDto exercise, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);

        if (!ModelState.IsValid)
        {
            var difficultyLevels = await uow.DifficultyLevels.GetAllAsync();
            var languages = await uow.Languages.GetAllAsync();
            var frameworks = await uow.Frameworks.GetAllAsync();

            ViewBag.DifficultyLevels = difficultyLevels;
            ViewBag.Languages = languages;
            ViewBag.Frameworks = frameworks;

            return View(exercise);
        }

        var exerciseEntity = exercise.Map();
        var id = await uow.Exercises.AddAsync(exerciseEntity);
        var solution = exerciseEntity.AuthorSolution;
        solution!.ExerciseId = id;
        await uow.Solutions.AddAsync(solution);
        await uow.CommitAsync(ct);

        logger.LogInformation(
            "Добавлено упражнение. Id:{Id}, Название:{Name}, Баллы:{Score}", 
            id, exercise.Name, exercise.Score);

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(ExerciseDto exercise, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        
        if (!ModelState.IsValid)
        {
            var difficultyLevels = await uow.DifficultyLevels.GetAllAsync();
            var languages = await uow.Languages.GetAllAsync();
            var frameworks = await uow.Frameworks.GetAllAsync();

            ViewBag.DifficultyLevels = difficultyLevels;
            ViewBag.Languages = languages;
            ViewBag.Frameworks = frameworks;

            return View(exercise);
        }
        
        var prev = await uow.Exercises.GetAsync(exercise.Id);
        if (prev is null)
            return NotFound();
        
        var solution = prev.AuthorSolution;
        solution!.S3Key = exercise.S3KeyAuthorSolution;
        await uow.Solutions.UpdateAsync(solution);
        
        if (prev.IsPublished)
            exercise.IsPublished = true;
        
        // Обновление упражнения
        await uow.Exercises.UpdateAsync(exercise.Map());
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