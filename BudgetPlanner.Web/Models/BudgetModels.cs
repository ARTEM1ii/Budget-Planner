using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Web.Models;

// Auth Models
public class LoginViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class RegisterViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

// Transaction Models
public class TransactionViewModel
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public int? CategoryId { get; set; }
    public CategoryViewModel? Category { get; set; }
}

public class CreateTransactionViewModel
{
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "Transaction type is required")]
    public TransactionType Type { get; set; }
    
    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; } = DateTime.Now;
    
    public int? CategoryId { get; set; }
    public List<CategoryViewModel> Categories { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

// Category Models
public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}

// Dashboard Models
public class DashboardViewModel
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public List<TransactionViewModel> RecentTransactions { get; set; } = new();
    public List<CategorySummaryViewModel> Categories { get; set; } = new();
}

public class CategorySummaryViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
}

public class CreateCategoryViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Color is required")]
    public string Color { get; set; } = "#007bff";
    
    [Required(ErrorMessage = "Type is required")]
    public TransactionType Type { get; set; }
    
    public string? ErrorMessage { get; set; }
}

// Enums
public enum TransactionType
{
    Income = 1,
    Expense = 2
}

// API Response Models
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
} 