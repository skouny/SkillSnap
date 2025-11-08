using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{    public class SkillService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7129/api/skills";

        public SkillService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get all skills from the API
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
        }

        /// <summary>
        /// Create a new skill
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
