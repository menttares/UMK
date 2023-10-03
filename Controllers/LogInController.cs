using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using УМК.Models;
namespace УМК.Controllers;


public class LogInController : Controller
{
    private readonly ILogger<LogInController> _logger;

    private Database _database;

    public LogInController(ILogger<LogInController> logger, Database database)
    {
        _database = database;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult AuthAdmin()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AuthAdmin(string Email, string Password, bool IsRememberMe)
    {
        Account? find = _database.Accounts.FirstOrDefault(ac => ac.Email == Email && ac.Password == Password);
        if (find is null)
            return Unauthorized();
        
        var claims = new List<Claim>
        {
        new Claim(ClaimsIdentity.DefaultNameClaimType, Email),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimsPrincipal);

        HttpContext.Response.Cookies.Append("Name", "Admin");
        HttpContext.Response.Cookies.Append("Role", "Admin");

        return Redirect("/Main/Index");
    }

    [HttpGet]
    public IActionResult AuthUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AuthUser(string NameUser, string Group, bool IsRememberMe)
    {

        var claims = new List<Claim>
        {
        new Claim(ClaimsIdentity.DefaultNameClaimType, NameUser),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimsPrincipal);

        HttpContext.Response.Cookies.Append("Name", NameUser);
        HttpContext.Response.Cookies.Append("Group", Group);
        HttpContext.Response.Cookies.Append("Role", "User");

        return Redirect("/Main/Index");
    }

    public IActionResult SignUp()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}
