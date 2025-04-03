using Coursework.Interfaces.Database;
using Coursework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

public class ExercisesController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        
        var difficultyLevels = await uow.DifficultyLevels.GetAllAsync();
        var frameworks = await uow.Frameworks.GetAllAsync();

        ViewBag.DifficultyLevels = difficultyLevels;
        ViewBag.Frameworks = frameworks;

        return View();
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

            return View(exercise);
        }

        var id = await uow.Exercises.AddAsync(exercise);
        await uow.CommitAsync(ct);

        logger.LogInformation("Добавлено упражнение. Id:{Id}, Название:{Name}, Баллы:{Score}", id, exercise.Name, exercise.Score);

        return RedirectToAction("Index", "Home");
    }
}