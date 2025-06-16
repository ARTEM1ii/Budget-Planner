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
public class CategoriesController : ControllerBase
{
    private readonly BudgetContext _context;

    public CategoriesController(BudgetContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var userId = GetCurrentUserId();
        var categories = await _context.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                Type = c.Type
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var userId = GetCurrentUserId();
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return NotFound();

        return Ok(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color,
            Type = category.Type
        });
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var userId = GetCurrentUserId();

        var category = new Category
        {
            Name = request.Name,
            Color = request.Color,
            Type = request.Type,
            UserId = userId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color,
            Type = category.Type
        };

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
    {
        var userId = GetCurrentUserId();
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return NotFound();

        category.Name = request.Name;
        category.Color = request.Color;
        category.Type = request.Type;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = GetCurrentUserId();
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return NotFound();

        var hasTransactions = await _context.Transactions
            .AnyAsync(t => t.CategoryId == id);

        if (hasTransactions)
            return BadRequest("Cannot delete category with existing transactions");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }
} 