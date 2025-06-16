using Microsoft.AspNetCore.Mvc;
using BudgetPlanner.Web.Models;
using BudgetPlanner.Web.Services;

namespace BudgetPlanner.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IApiService _apiService;

    public DashboardController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var dashboardData = await _apiService.GetDashboardDataAsync();
        return View(dashboardData ?? new DashboardViewModel());
    }

    public async Task<IActionResult> Transactions()
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var transactions = await _apiService.GetTransactionsAsync();
        return View(transactions);
    }

    [HttpGet]
    public async Task<IActionResult> CreateTransaction()
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var categories = await _apiService.GetCategoriesAsync();
        var model = new CreateTransactionViewModel
        {
            Categories = categories,
            Date = DateTime.Now
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionViewModel model)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await _apiService.GetCategoriesAsync();
            return View(model);
        }

        var success = await _apiService.CreateTransactionAsync(model);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Transaction created successfully";
            return RedirectToAction("Transactions");
        }

        TempData["ErrorMessage"] = "Error creating transaction";
        model.Categories = await _apiService.GetCategoriesAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditTransaction(int id)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var transaction = await _apiService.GetTransactionByIdAsync(id);
        if (transaction == null)
        {
            TempData["ErrorMessage"] = "Transaction not found";
            return RedirectToAction("Transactions");
        }

        var categories = await _apiService.GetCategoriesAsync();
        var model = new CreateTransactionViewModel
        {
            Description = transaction.Description,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Date = transaction.Date,
            CategoryId = transaction.CategoryId,
            Categories = categories
        };

        ViewBag.TransactionId = id;
        ViewBag.IsEdit = true;
        return View("CreateTransaction", model);
    }

    [HttpPost]
    public async Task<IActionResult> EditTransaction(int id, CreateTransactionViewModel model)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await _apiService.GetCategoriesAsync();
            ViewBag.TransactionId = id;
            ViewBag.IsEdit = true;
            return View("CreateTransaction", model);
        }

        var success = await _apiService.UpdateTransactionAsync(id, model);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Transaction updated successfully";
            return RedirectToAction("Transactions");
        }

        TempData["ErrorMessage"] = "Error updating transaction";
        model.Categories = await _apiService.GetCategoriesAsync();
        ViewBag.TransactionId = id;
        ViewBag.IsEdit = true;
        return View("CreateTransaction", model);
    }

    public async Task<IActionResult> Categories()
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var categories = await _apiService.GetCategoriesAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult CreateCategory()
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = new CreateCategoryViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var success = await _apiService.CreateCategoryAsync(model);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Category created successfully";
            return RedirectToAction("Categories");
        }

        TempData["ErrorMessage"] = "Error creating category";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var category = await _apiService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            TempData["ErrorMessage"] = "Category not found";
            return RedirectToAction("Categories");
        }

        var model = new CreateCategoryViewModel
        {
            Name = category.Name,
            Color = category.Color,
            Type = category.Type
        };

        ViewBag.CategoryId = id;
        ViewBag.IsEdit = true;
        return View("CreateCategory", model);
    }

    [HttpPost]
    public async Task<IActionResult> EditCategory(int id, CreateCategoryViewModel model)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.CategoryId = id;
            ViewBag.IsEdit = true;
            return View("CreateCategory", model);
        }

        var success = await _apiService.UpdateCategoryAsync(id, model);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Category updated successfully";
            return RedirectToAction("Categories");
        }

        TempData["ErrorMessage"] = "Error updating category";
        ViewBag.CategoryId = id;
        ViewBag.IsEdit = true;
        return View("CreateCategory", model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var success = await _apiService.DeleteTransactionAsync(id);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Transaction deleted successfully";
        }
        else
        {
            TempData["ErrorMessage"] = "Error deleting transaction";
        }

        return RedirectToAction("Transactions");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!_apiService.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var success = await _apiService.DeleteCategoryAsync(id);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Category deleted successfully";
        }
        else
        {
            TempData["ErrorMessage"] = "Error deleting category";
        }

        return RedirectToAction("Categories");
    }
} 