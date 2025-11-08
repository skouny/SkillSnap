using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{
    /// <summary>
    /// Service for managing skill data through the API
    /// Handles CRUD operations for skills with proper error handling
    /// </summary>
    public class SkillService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7129/api/skills";

        public SkillService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get all skills from the API
        /// Returns empty list on error to prevent null reference exceptions
        /// </summary>
        public async Task<List<Skill>> GetSkillsAsync()
        {
            try
            {
                var skills = await _httpClient.GetFromJsonAsync<List<Skill>>(ApiBaseUrl);
                return skills ?? new List<Skill>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching skills: {ex.Message}");
                return new List<Skill>();
            }
        }

        /// <summary>
        /// Alias for GetSkillsAsync to maintain backward compatibility
        /// </summary>
        public Task<List<Skill>> GetAllSkillsAsync() => GetSkillsAsync();

        /// <summary>
        /// Get a specific skill by ID
        /// </summary>
        public async Task<Skill?> GetSkillByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Skill>($"{ApiBaseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching skill {id}: {ex.Message}");
                return null;
            }
        }        /// <summary>
        /// Create a new skill
        /// Returns the created skill with its assigned ID, or null on failure
        /// </summary>
        public async Task<Skill?> CreateSkillAsync(Skill newSkill)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, newSkill);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Skill>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating skill: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Add a new skill (legacy method for backward compatibility)
        /// </summary>
        public async Task<bool> AddSkillAsync(Skill newSkill)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, newSkill);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding skill: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update an existing skill
        /// </summary>
        public async Task<bool> UpdateSkillAsync(int id, Skill skill)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{id}", skill);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating skill: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a skill
        /// </summary>
        public async Task<bool> DeleteSkillAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting skill: {ex.Message}");
                return false;
            }
        }
    }
}
