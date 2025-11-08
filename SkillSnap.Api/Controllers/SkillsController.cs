using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SkillsController> _logger;
        private const string SkillsCacheKey = "skills_list";

        public SkillsController(SkillSnapContext context, IMemoryCache cache, ILogger<SkillsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }        /// <summary>
        /// Get all skills with their associated portfolio user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            try
            {
                // Try to get from cache
                if (!_cache.TryGetValue(SkillsCacheKey, out List<Skill>? skills))
                {
                    _logger.LogInformation("Cache miss - fetching skills from database");
                    
                    // Cache miss - fetch from database with optimized query
                    skills = await _context.Skills
                        .Include(s => s.PortfolioUser)
                        .AsNoTracking() // Optimization: no change tracking needed for read-only queries
                        .ToListAsync();
                    
                    // Set cache with 5 minute expiration
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };
                    
                    _cache.Set(SkillsCacheKey, skills, cacheOptions);
                }
                else
                {
                    _logger.LogInformation("Cache hit - returning cached skills");
                }

                return Ok(skills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching skills");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Get a specific skill by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            try
            {
                var skill = await _context.Skills
                    .Include(s => s.PortfolioUser)
                    .AsNoTracking() // Optimization: no change tracking needed
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (skill == null)
                {
                    return NotFound($"Skill with ID {id} not found.");
                }

                return Ok(skill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching skill {SkillId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Create a new skill
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
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
                    .AnyAsync(u => u.Id == skill.PortfolioUserId);

                if (!userExists)
                {
                    return BadRequest("Invalid PortfolioUserId. User does not exist.");
                }

                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();

                // Invalidate cache after creating
                _cache.Remove(SkillsCacheKey);
                _logger.LogInformation("Cache invalidated after creating skill {SkillId}", skill.Id);

                return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating skill");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Update an existing skill
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSkill(int id, Skill skill)
        {
            try
            {
                if (id != skill.Id)
                {
                    return BadRequest("Skill ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingSkill = await _context.Skills.FindAsync(id);
                if (existingSkill == null)
                {
                    return NotFound($"Skill with ID {id} not found.");
                }

                existingSkill.Name = skill.Name;
                existingSkill.Level = skill.Level;
                existingSkill.PortfolioUserId = skill.PortfolioUserId;

                await _context.SaveChangesAsync();

                // Invalidate cache after updating
                _cache.Remove(SkillsCacheKey);
                _logger.LogInformation("Cache invalidated after updating skill {SkillId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating skill {SkillId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Delete a skill
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);
                if (skill == null)
                {
                    return NotFound($"Skill with ID {id} not found.");
                }

                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();

                // Invalidate cache after deleting
                _cache.Remove(SkillsCacheKey);
                _logger.LogInformation("Cache invalidated after deleting skill {SkillId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting skill {SkillId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
