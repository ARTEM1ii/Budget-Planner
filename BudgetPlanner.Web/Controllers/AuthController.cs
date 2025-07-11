using Microsoft.AspNetCore.Mvc;
using BudgetPlanner.Web.Models;
using BudgetPlanner.Web.Services;

namespace BudgetPlanner.Web.Controllers;

public class AuthController : Controller
{
    private readonly IApiService _apiService;

    public AuthController(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (_apiService.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _apiService.LoginAsync(model.Username, model.Password);
        
        if (result != null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        model.ErrorMessage = "Invalid username or password";
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (_apiService.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Password != model.ConfirmPassword)
        {
            model.ErrorMessage = "Passwords do not match";
            return View(model);
        }

        var result = await _apiService.RegisterAsync(model.Username, model.Email, model.Password);
        
        if (result != null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        model.ErrorMessage = "User with this username or email already exists";
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
} 