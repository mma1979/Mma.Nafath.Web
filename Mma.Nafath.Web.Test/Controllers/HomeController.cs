using Microsoft.AspNetCore.Mvc;
using Mma.Nafath.Web.Test.Models;
using System.Diagnostics;
using Mma.Nafath.Web.Services;
using Mma.Nafath.Web.Models;

namespace Mma.Nafath.Web.Test.Controllers;

public class HomeController : Controller
{
    private readonly INafathWebService _nafathService;

    public HomeController(INafathWebService nafathService)
    {
        _nafathService = nafathService;
    }
    public IActionResult Index()
    {
        return View();
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

    public async Task<IActionResult> StartLogin(string nationalId)
    {
        var result = await _nafathService.CreateSessionAsync();
        return Redirect(result?.RedirectUrl ?? "/");
    }
}
