using Microsoft.AspNetCore.Mvc;
using BudgetPlanner.API.Models;
using BudgetPlanner.API.Services;

namespace BudgetPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            return BadRequest("Username and password are required");

        var token = await _authService.AuthenticateAsync(request.Username, request.Password);
        if (token == null)
            return Unauthorized("Invalid credentials");

        var user = await _authService.GetUserByIdAsync(GetUserIdFromToken(token));
        if (user == null)
            return NotFound("User not found");

        return Ok(new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt
            }
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || 
            string.IsNullOrEmpty(request.Email) || 
            string.IsNullOrEmpty(request.Password))
            return BadRequest("All fields are required");

        if (request.Password.Length < 6)
            return BadRequest("Password must be at least 6 characters");

        var user = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        if (user == null)
            return Conflict("Username or email already exists");

        var token = await _authService.AuthenticateAsync(user.Username, request.Password);
        if (token == null)
            return StatusCode(500, "Registration succeeded but login failed");

        return Ok(new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt
            }
        });
    }

    private static int GetUserIdFromToken(string token)
    {
        // Simplified - in real app would decode JWT
        return 1; // For now return default
    }
} 