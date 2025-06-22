using System.Diagnostics;
using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using MVC.Models;

namespace MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDb<Course, int> _courseContext;
    private readonly IDb<Forum, int> _forumContext;
    private readonly IDb<Battle, int> _battleContext;

    public HomeController(ILogger<HomeController> logger, CoursesContext coursesContext, ForumContext forumContext, BattlesContext battlesContext)
    {
        _logger = logger;
        _courseContext = coursesContext;
        _forumContext = forumContext;
        _battleContext = battlesContext;
    }

    public async Task<IActionResult> Index()
    {
        var homeViewModel = new HomeViewModel();
        homeViewModel.Battles = (await _battleContext.ReadAll(true, true)).TakeLast(3).ToList();
        homeViewModel.Forums = (await _forumContext.ReadAll(true, true)).TakeLast(3).ToList();
        homeViewModel.Courses = (await _courseContext.ReadAll(true, true)).TakeLast(3).ToList();
        return View(homeViewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
