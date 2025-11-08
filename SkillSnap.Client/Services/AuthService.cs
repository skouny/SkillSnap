using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private const string ApiBaseUrl = "https://localhost:7129/api/auth";
        private const string TokenKey = "authToken";

        public event Action? OnAuthStateChanged;
        public bool IsAuthenticated { get; private set; }
        public string? CurrentUserEmail { get; private set; }
        public string? CurrentUserName { get; private set; }

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                SetAuthorizationHeader(token);
                IsAuthenticated = true;
                await LoadUserInfo();
            }
        }        public async Task<(bool success, string? error)> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                Console.WriteLine($"Attempting login to: {ApiBaseUrl}/login");
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (loginResponse != null)
                    {
                        await SaveTokenAsync(loginResponse.Token);
                        SetAuthorizationHeader(loginResponse.Token);
                        CurrentUserEmail = loginResponse.Email;
                        CurrentUserName = loginResponse.FullName;
                        IsAuthenticated = true;
                        NotifyAuthStateChanged();
                        Console.WriteLine($"Login successful for user: {loginResponse.Email}");
                        return (true, null);
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login failed with status: {response.StatusCode}, Error: {errorContent}");
                return (false, $"Login failed: {errorContent}");
            }
            catch (HttpRequestException ex)
            {
                var errorMsg = $"Cannot connect to API at {ApiBaseUrl}. Make sure the API is running. Error: {ex.Message}";
                Console.WriteLine($"Login HTTP error: {errorMsg}");
                return (false, errorMsg);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Login failed: {ex.GetType().Name}: {ex.Message}";
                Console.WriteLine($"Login error: {errorMsg}");
                return (false, errorMsg);
            }
        }public async Task<(bool success, string? error)> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                Console.WriteLine($"Attempting registration to: {ApiBaseUrl}/register");
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/register", registerRequest);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Registration successful");
                    return (true, null);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Registration failed with status: {response.StatusCode}, Error: {errorContent}");
                return (false, $"Registration failed: {errorContent}");
            }
            catch (HttpRequestException ex)
            {
                var errorMsg = $"Cannot connect to API at {ApiBaseUrl}. Make sure the API is running. Error: {ex.Message}";
                Console.WriteLine($"Registration HTTP error: {errorMsg}");
                return (false, errorMsg);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Registration failed: {ex.GetType().Name}: {ex.Message}";
                Console.WriteLine($"Registration error: {errorMsg}");
                return (false, errorMsg);
            }
        }

        public async Task LogoutAsync()
        {
            await RemoveTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            IsAuthenticated = false;
            CurrentUserEmail = null;
            CurrentUserName = null;
            NotifyAuthStateChanged();
        }

        private async Task LoadUserInfo()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<LoginResponse>($"{ApiBaseUrl}/me");
                if (response != null)
                {
                    CurrentUserEmail = response.Email;
                    CurrentUserName = response.FullName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user info: {ex.Message}");
            }
        }

        private void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            }
            catch
            {
                return null;
            }
        }

        private async Task SaveTokenAsync(string token)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving token: {ex.Message}");
            }
        }

        private async Task RemoveTokenAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing token: {ex.Message}");
            }
        }

        private void NotifyAuthStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}
