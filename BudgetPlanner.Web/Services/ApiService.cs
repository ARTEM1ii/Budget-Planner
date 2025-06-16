using System.Text;
using System.Text.Json;
using BudgetPlanner.Web.Models;

namespace BudgetPlanner.Web.Services;

public interface IApiService
{
    Task<AuthResponse?> LoginAsync(string username, string password);
    Task<AuthResponse?> RegisterAsync(string username, string email, string password);
    Task<List<TransactionViewModel>> GetTransactionsAsync();
    Task<TransactionViewModel?> GetTransactionByIdAsync(int id);
    Task<bool> UpdateTransactionAsync(int id, CreateTransactionViewModel transaction);
    Task<bool> DeleteTransactionAsync(int id);
    Task<List<CategoryViewModel>> GetCategoriesAsync();
    Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
    Task<bool> CreateCategoryAsync(CreateCategoryViewModel category);
    Task<bool> UpdateCategoryAsync(int id, CreateCategoryViewModel category);
    Task<bool> DeleteCategoryAsync(int id);
    Task<DashboardViewModel?> GetDashboardDataAsync();
    Task<bool> CreateTransactionAsync(CreateTransactionViewModel transaction);
    void SetAuthToken(string token);
    bool IsAuthenticated { get; }
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(GetAuthToken());

    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        var loginRequest = new { username, password };
        var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/auth/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, _jsonOptions);
            
            if (authResponse != null)
            {
                SetAuthToken(authResponse.Token);
                return authResponse;
            }
        }

        return null;
    }

    public async Task<AuthResponse?> RegisterAsync(string username, string email, string password)
    {
        var registerRequest = new { username, email, password };
        var json = JsonSerializer.Serialize(registerRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/auth/register", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, _jsonOptions);
            
            if (authResponse != null)
            {
                SetAuthToken(authResponse.Token);
                return authResponse;
            }
        }

        return null;
    }

    public async Task<List<TransactionViewModel>> GetTransactionsAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("/api/transactions");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<TransactionViewModel>>(json, _jsonOptions);
            return transactions ?? new List<TransactionViewModel>();
        }

        return new List<TransactionViewModel>();
    }

    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("/api/categories");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(json, _jsonOptions);
            return categories ?? new List<CategoryViewModel>();
        }

        return new List<CategoryViewModel>();
    }

    public async Task<DashboardViewModel?> GetDashboardDataAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("/api/transactions/summary");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var summary = JsonSerializer.Deserialize<DashboardViewModel>(json, _jsonOptions);
            
            if (summary != null)
            {
                // Получаем последние транзакции
                summary.RecentTransactions = (await GetTransactionsAsync()).Take(5).ToList();
                return summary;
            }
        }

        return null;
    }

    public async Task<bool> CreateTransactionAsync(CreateTransactionViewModel transaction)
    {
        SetAuthHeader();
        
        var createRequest = new 
        {
            description = transaction.Description,
            amount = transaction.Amount,
            type = transaction.Type,
            date = transaction.Date,
            categoryId = transaction.CategoryId
        };

        var json = JsonSerializer.Serialize(createRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/transactions", content);
        return response.IsSuccessStatusCode;
    }

    public void SetAuthToken(string token)
    {
        _httpContextAccessor.HttpContext?.Session.SetString("AuthToken", token);
    }

    private string? GetAuthToken()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
    }

    public async Task<TransactionViewModel?> GetTransactionByIdAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"/api/transactions/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TransactionViewModel>(json, _jsonOptions);
        }

        return null;
    }

    public async Task<bool> UpdateTransactionAsync(int id, CreateTransactionViewModel transaction)
    {
        SetAuthHeader();
        
        var updateRequest = new 
        {
            description = transaction.Description,
            amount = transaction.Amount,
            type = transaction.Type,
            date = transaction.Date,
            categoryId = transaction.CategoryId
        };

        var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"/api/transactions/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"/api/transactions/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"/api/categories/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryViewModel>(json, _jsonOptions);
        }

        return null;
    }

    public async Task<bool> CreateCategoryAsync(CreateCategoryViewModel category)
    {
        SetAuthHeader();
        
        var createRequest = new 
        {
            name = category.Name,
            color = category.Color,
            type = category.Type
        };

        var json = JsonSerializer.Serialize(createRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/categories", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCategoryAsync(int id, CreateCategoryViewModel category)
    {
        SetAuthHeader();
        
        var updateRequest = new 
        {
            name = category.Name,
            color = category.Color,
            type = category.Type
        };

        var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"/api/categories/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"/api/categories/{id}");
        return response.IsSuccessStatusCode;
    }

    private void SetAuthHeader()
    {
        var token = GetAuthToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
} 