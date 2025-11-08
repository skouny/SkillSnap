using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProjectsController> _logger;
        private const string ProjectsCacheKey = "projects_list";

        public ProjectsController(SkillSnapContext context, IMemoryCache cache, ILogger<ProjectsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }        /// <summary>
        /// Get all projects with their associated portfolio user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            try
            {
                // Try to get from cache
                if (!_cache.TryGetValue(ProjectsCacheKey, out List<Project>? projects))
                {
                    _logger.LogInformation("Cache miss - fetching projects from database");
                    
                    // Cache miss - fetch from database with optimized query
                    projects = await _context.Projects
                        .Include(p => p.PortfolioUser)
                        .AsNoTracking() // Optimization: no change tracking needed for read-only queries
                        .ToListAsync();
                    
                    // Set cache with 5 minute expiration
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };
                    
                    _cache.Set(ProjectsCacheKey, projects, cacheOptions);
                }
                else
                {
                    _logger.LogInformation("Cache hit - returning cached projects");
                }

                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching projects");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Get a specific project by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.PortfolioUser)
                    .AsNoTracking() // Optimization: no change tracking needed
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    return NotFound($"Project with ID {id} not found.");
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching project {ProjectId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify that the PortfolioUser exists
                var userExists = await _context.PortfolioUsers
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == project.PortfolioUserId);

                if (!userExists)
                {
                    return BadRequest("Invalid PortfolioUserId. User does not exist.");
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                // Invalidate cache after creating
                _cache.Remove(ProjectsCacheKey);
                _logger.LogInformation("Cache invalidated after creating project {ProjectId}", project.Id);

                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Update an existing project
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            try
            {
                if (id != project.Id)
                {
                    return BadRequest("Project ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingProject = await _context.Projects.FindAsync(id);
                if (existingProject == null)
                {
                    return NotFound($"Project with ID {id} not found.");
                }

                existingProject.Title = project.Title;
                existingProject.Description = project.Description;
                existingProject.ImageUrl = project.ImageUrl;
                existingProject.PortfolioUserId = project.PortfolioUserId;

                await _context.SaveChangesAsync();

                // Invalidate cache after updating
                _cache.Remove(ProjectsCacheKey);
                _logger.LogInformation("Cache invalidated after updating project {ProjectId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Delete a project
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);
                if (project == null)
                {
                    return NotFound($"Project with ID {id} not found.");
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                // Invalidate cache after deleting
                _cache.Remove(ProjectsCacheKey);
                _logger.LogInformation("Cache invalidated after deleting project {ProjectId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
