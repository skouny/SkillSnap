using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly SkillSnapContext _context;

        public SkillsController(SkillSnapContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all skills with their associated portfolio user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            try
            {
                var skills = await _context.Skills
                    .Include(s => s.PortfolioUser)
                    .ToListAsync();
                return Ok(skills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a specific skill by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            try
            {
                var skill = await _context.Skills
                    .Include(s => s.PortfolioUser)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (skill == null)
                {
                    return NotFound($"Skill with ID {id} not found.");
                }

                return Ok(skill);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
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
                    .AnyAsync(u => u.Id == skill.PortfolioUserId);

                if (!userExists)
                {
                    return BadRequest("Invalid PortfolioUserId. User does not exist.");
                }

                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
