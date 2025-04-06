using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coursework.Models;
using Coursework.Models.DTOs;
using Coursework.Models.Entities;

namespace Coursework.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}