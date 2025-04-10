using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coursework.Models;
using Coursework.Models.DTOs;
using Coursework.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Coursework.Controllers;

[Authorize(Policy = "UserAccess")]
public class HomeController(ILogger<HomeController> logger) : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}