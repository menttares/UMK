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

[Authorize]
public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;

    private Database _Database;

    public MainController(ILogger<MainController> logger, Database Database)
    {
        _logger = logger;
        _Database = Database;
    }

    
    public IActionResult Index()
    {
        return View();
    }
}
