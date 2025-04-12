using Coursework.Extensions;
using Coursework.Interfaces.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public UserController(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    [HttpGet]
    public async Task<IActionResult> View(long id, CancellationToken ct)
    {
        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync(ct);
        var user = await unitOfWork.Users.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
}