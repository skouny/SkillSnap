using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly SkillSnapContext _context;

        public ProjectsController(SkillSnapContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all projects with their associated portfolio user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            try
            {
                var projects = await _context.Projects
                    .Include(p => p.PortfolioUser)
                    .ToListAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a specific project by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.PortfolioUser)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    return NotFound($"Project with ID {id} not found.");
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
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
                    .AnyAsync(u => u.Id == project.PortfolioUserId);

                if (!userExists)
                {
                    return BadRequest("Invalid PortfolioUserId. User does not exist.");
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (Exception ex)
            {
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

                return NoContent();
            }
            catch (Exception ex)
            {
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
