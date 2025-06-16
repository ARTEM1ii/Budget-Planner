using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BudgetPlanner.API.Models;
using BudgetPlanner.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.API.Services;

public interface IAuthService
{
    Task<string?> AuthenticateAsync(string username, string password);
    Task<User?> RegisterAsync(string username, string email, string password);
    Task<User?> GetUserByIdAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly BudgetContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(BudgetContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return GenerateJwtToken(user);
    }

    public async Task<User?> RegisterAsync(string username, string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
            return null;

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Создаем базовые категории для нового пользователя
        await CreateDefaultCategoriesAsync(user.Id);

        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "YourSecretKeyHere123456789");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task CreateDefaultCategoriesAsync(int userId)
    {
        var defaultCategories = new List<Category>
        {
            new() { Name = "Зарплата", Color = "#28a745", Type = TransactionType.Income, UserId = userId },
            new() { Name = "Продукты", Color = "#dc3545", Type = TransactionType.Expense, UserId = userId },
            new() { Name = "Транспорт", Color = "#ffc107", Type = TransactionType.Expense, UserId = userId },
            new() { Name = "Развлечения", Color = "#17a2b8", Type = TransactionType.Expense, UserId = userId }
        };

        _context.Categories.AddRange(defaultCategories);
        await _context.SaveChangesAsync();
    }
} 