using Coursework.Extensions;
using Coursework.Interfaces.Database;
using Coursework.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Coursework.Controllers;

[Authorize(Policy = "UserAccess")]
public class ExercisesController(IUnitOfWorkFactory uowFactory, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(string search, CancellationToken ct)
    {
        await using var uow = await uowFactory.CreateAsync(ct);
        var exercises = await uow.Exercises.GetAllAsync(search);

        var currentUserId = User.GetId();
        var exerciseDtos = exercises.Select(e => e.MapWithCurrentUser(currentUserId)).ToList();

        ViewBag.Exercises = exerciseDtos.Where(x => x.IsPublished 
                                                    || x.AuthorId == User.GetId() 
                                                    || User.IsInRole("Администратор"));
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
        if (exercise.AuthorId != User.GetId() && !User.IsInRole("Администратор"))
            return Forbid();

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

        var currentUserId = User.GetId();
        return View(exercise.MapWithCurrentUser(currentUserId));
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

        exercise.AuthorId = User.GetId();
        var exerciseEntity = exercise.Map();
        var id = await uow.Exercises.AddAsync(exerciseEntity);
        var solution = exerciseEntity.AuthorSolution;
        solution!.ExerciseId = id;
        await uow.Solutions.AddAsync(solution);
        await uow.CommitAsync(ct);

        logger.LogInformation(
            "Добавлено упражнение. Id:{Id}, Название:{Name}, Баллы:{Score}, Автор: {AuthorId}",
            id, exercise.Name, exercise.Score, exerciseEntity.AuthorId);

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
        if (prev.AuthorId != User.GetId() && !User.IsInRole("Администратор"))
            return Forbid();

        var solution = prev.AuthorSolution;
        solution!.S3Key = exercise.S3KeyAuthorSolution;
        await uow.Solutions.UpdateAsync(solution);

        if (prev.IsPublished)
            exercise.IsPublished = true;

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
        var exercise = await uow.Exercises.GetAsync(id);
        if (exercise is null)
            return NotFound();
        if (exercise.AuthorId != User.GetId() && !User.IsInRole("Администратор"))
            return Forbid();
        await uow.Exercises.DeleteAsync(id);
        await uow.CommitAsync(ct);

        logger.LogInformation("Удалено упражнение. Id:{Id}", id);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Like(long id, string returnUrl, CancellationToken ct)
    {
        var userId = User.GetId();

        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Exercises.AddLikeAsync(id, userId);
        await uow.CommitAsync(ct);

        logger.LogInformation("Пользователь {UserId} лайкнул упражнение {ExerciseId}", userId, id);

        if (string.IsNullOrEmpty(returnUrl))
            return RedirectToAction("Index");

        return Redirect(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Unlike(long id, string returnUrl, CancellationToken ct)
    {
        var userId = User.GetId();

        await using var uow = await uowFactory.CreateAsync(ct);
        await uow.Exercises.RemoveLikeAsync(id, userId);
        await uow.CommitAsync(ct);

        logger.LogInformation("Пользователь {UserId} убрал лайк с упражнения {ExerciseId}", userId, id);

        if (string.IsNullOrEmpty(returnUrl))
            return RedirectToAction("Index");

        return Redirect(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> OpenHint(long id, long exerciseId, CancellationToken ct)
    {
        var userId = User.GetId();

        await using var uow = await uowFactory.CreateAsync(ct);
        
        var exercise = await uow.Exercises.GetAsync(exerciseId);
        if (exercise == null)
            return NotFound();

        var hint = exercise.Hints.FirstOrDefault(h => h.Id == id);
        if (hint == null)
            return NotFound();
        
        var user = await uow.Users.GetAsync(userId);
        if (user == null)
            return NotFound();

        if (!User.IsInRole("Администратор"))
        {
            if (user.Score < hint.Cost)
            {
                TempData["ErrorMessage"] =
                    $"У вас недостаточно очков для открытия этой подсказки. Требуется: {hint.Cost}, у вас: {user.Score}";
                return RedirectToAction("View", new { id = exerciseId });
            }
        
            user.Score -= hint.Cost;
            await uow.Users.UpdateAsync(user);
        }
        
        await uow.Exercises.OpenHintAsync(id, userId);
        await uow.CommitAsync(ct);

        logger.LogInformation("Пользователь {UserId} открыл подсказку {HintId} за {Cost} очков", userId, id, hint.Cost);

        return RedirectToAction("View", new { id = exerciseId });
    }
}