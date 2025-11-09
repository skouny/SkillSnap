using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{
    /// <summary>
    /// Service for managing project data through the API
    /// Handles CRUD operations for projects with proper error handling    /// </summary>
    public class ProjectService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5064/api/projects";

        public ProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get all projects from the API
        /// Returns empty list on error to prevent null reference exceptions
        /// </summary>
        public async Task<List<Project>> GetProjectsAsync()
        {
            try
            {
                var projects = await _httpClient.GetFromJsonAsync<List<Project>>(ApiBaseUrl);
                return projects ?? new List<Project>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching projects: {ex.Message}");
                return new List<Project>();
            }
        }

        /// <summary>
        /// Alias for GetProjectsAsync to maintain backward compatibility
        /// </summary>
        public Task<List<Project>> GetAllProjectsAsync() => GetProjectsAsync();

        /// <summary>
        /// Get a specific project by ID
        /// </summary>
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Project>($"{ApiBaseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching project {id}: {ex.Message}");
                return null;
            }
        }        /// <summary>
        /// Create a new project
        /// Returns the created project with its assigned ID, or null on failure
        /// </summary>
        public async Task<Project?> CreateProjectAsync(Project newProject)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, newProject);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Project>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating project: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Add a new project (legacy method for backward compatibility)
        /// </summary>
        public async Task<bool> AddProjectAsync(Project newProject)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, newProject);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding project: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        public async Task<bool> UpdateProjectAsync(int id, Project project)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{id}", project);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating project: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        public async Task<bool> DeleteProjectAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting project: {ex.Message}");
                return false;
            }
        }
    }
}
