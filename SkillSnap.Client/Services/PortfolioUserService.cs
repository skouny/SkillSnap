using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{    public class PortfolioUserService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7129/api/portfoliousers";

        public PortfolioUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }        /// <summary>
        /// Get all portfolio users from the API
        /// </summary>
        public async Task<List<PortfolioUser>> GetUsersAsync()
        {
            try
            {
                Console.WriteLine($"Fetching users from: {ApiBaseUrl}");
                var users = await _httpClient.GetFromJsonAsync<List<PortfolioUser>>(ApiBaseUrl);
                Console.WriteLine($"Received {users?.Count ?? 0} users");
                return users ?? new List<PortfolioUser>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<PortfolioUser>();
            }
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        public async Task<PortfolioUser?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PortfolioUser>($"{ApiBaseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user {id}: {ex.Message}");
                return null;
            }
        }        /// <summary>
        /// Get the first user (useful for single-user portfolio)
        /// </summary>
        public async Task<PortfolioUser?> GetFirstUserAsync()
        {
            try
            {
                Console.WriteLine("Getting first user...");
                var users = await GetUsersAsync();
                var firstUser = users.FirstOrDefault();
                Console.WriteLine($"First user: {firstUser?.Name ?? "null"}");
                return firstUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching first user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
