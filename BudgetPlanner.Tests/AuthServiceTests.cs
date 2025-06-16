using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BudgetPlanner.API.Data;
using BudgetPlanner.API.Services;
using BudgetPlanner.API.Models;

namespace BudgetPlanner.Tests;

public class AuthServiceTests : IDisposable
{
    private readonly BudgetContext _context;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<BudgetContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BudgetContext(options);
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestSecretKey123456789012345678901234567890"
            })
            .Build();

        _authService = new AuthService(_context, configuration);
    }

    [Fact]
    public async Task RegisterAsync_ValidUser_ReturnsUser()
    {
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        var result = await _authService.RegisterAsync(username, email, password);

        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.Equal(email, result.Email);
        Assert.False(result.IsAdmin);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateUsername_ReturnsNull()
    {
        var username = "testuser";
        var email1 = "test1@example.com";
        var email2 = "test2@example.com";
        var password = "password123";

        await _authService.RegisterAsync(username, email1, password);

        var result = await _authService.RegisterAsync(username, email2, password);

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
    {
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        await _authService.RegisterAsync(username, email, password);

        var token = await _authService.AuthenticateAsync(username, password);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidCredentials_ReturnsNull()
    {
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";
        var wrongPassword = "wrongpassword";

        await _authService.RegisterAsync(username, email, password);


        var token = await _authService.AuthenticateAsync(username, wrongPassword);


        Assert.Null(token);
    }

    [Fact]
    public async Task AuthenticateAsync_WithEmail_ReturnsToken()
    {

        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        await _authService.RegisterAsync(username, email, password);


        var token = await _authService.AuthenticateAsync(email, password);


        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 