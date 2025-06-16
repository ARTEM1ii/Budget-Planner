using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BudgetPlanner.API.Data;
using BudgetPlanner.API.Models;
using System.Security.Claims;

namespace BudgetPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly BudgetContext _context;

    public TransactionsController(BudgetContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
    {
        var userId = GetCurrentUserId();
        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Description = t.Description,
                Amount = t.Amount,
                Type = t.Type,
                Date = t.Date,
                Category = t.Category != null ? new CategoryDto
                {
                    Id = t.Category.Id,
                    Name = t.Category.Name,
                    Color = t.Category.Color,
                    Type = t.Category.Type
                } : null
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
    {
        var userId = GetCurrentUserId();
        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null)
            return NotFound();

        return Ok(new TransactionDto
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Date = transaction.Date,
            Category = transaction.Category != null ? new CategoryDto
            {
                Id = transaction.Category.Id,
                Name = transaction.Category.Name,
                Color = transaction.Category.Color,
                Type = transaction.Category.Type
            } : null
        });
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        var userId = GetCurrentUserId();

        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId && c.UserId == userId);
            if (!categoryExists)
                return BadRequest("Category not found or doesn't belong to user");
        }

        var transaction = new Transaction
        {
            Description = request.Description,
            Amount = request.Amount,
            Type = request.Type,
            Date = request.Date,
            UserId = userId,
            CategoryId = request.CategoryId
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var createdTransaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstAsync(t => t.Id == transaction.Id);

        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id },
            new TransactionDto
            {
                Id = createdTransaction.Id,
                Description = createdTransaction.Description,
                Amount = createdTransaction.Amount,
                Type = createdTransaction.Type,
                Date = createdTransaction.Date,
                Category = createdTransaction.Category != null ? new CategoryDto
                {
                    Id = createdTransaction.Category.Id,
                    Name = createdTransaction.Category.Name,
                    Color = createdTransaction.Category.Color,
                    Type = createdTransaction.Category.Type
                } : null
            });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] CreateTransactionRequest request)
    {
        var userId = GetCurrentUserId();
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null)
            return NotFound();

        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId && c.UserId == userId);
            if (!categoryExists)
                return BadRequest("Category not found or doesn't belong to user");
        }

        transaction.Description = request.Description;
        transaction.Amount = request.Amount;
        transaction.Type = request.Type;
        transaction.Date = request.Date;
        transaction.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var userId = GetCurrentUserId();
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null)
            return NotFound();

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("summary")]
    public async Task<ActionResult<BudgetSummaryDto>> GetBudgetSummary()
    {
        var userId = GetCurrentUserId();
        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var categoryGroups = transactions
            .Where(t => t.Category != null)
            .GroupBy(t => t.Category!)
            .Select(g => new CategorySummaryDto
            {
                Name = g.Key.Name,
                Color = g.Key.Color,
                Type = g.Key.Type,
                Amount = g.Sum(t => t.Amount)
            })
            .ToList();

        return Ok(new BudgetSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses,
            Categories = categoryGroups
        });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }
} 