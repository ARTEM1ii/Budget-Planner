namespace BudgetPlanner.API.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public int? CategoryId { get; set; }
    
    public User User { get; set; } = null!;
    public Category? Category { get; set; }
}

public enum TransactionType
{
    Income = 1,
    Expense = 2
} 