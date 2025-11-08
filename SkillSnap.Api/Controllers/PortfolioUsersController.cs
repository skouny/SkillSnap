using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioUsersController : ControllerBase
    {
        private readonly SkillSnapContext _context;

        public PortfolioUsersController(SkillSnapContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all portfolio users with their projects and skills
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioUser>>> GetUsers()
        {
            try
            {
                var users = await _context.PortfolioUsers
                    .Include(u => u.Projects)
                    .Include(u => u.Skills)
                    .ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a specific user by ID with their projects and skills
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioUser>> GetUser(int id)
        {
            try
            {
                var user = await _context.PortfolioUsers
                    .Include(u => u.Projects)
                    .Include(u => u.Skills)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new portfolio user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PortfolioUser>> CreateUser(PortfolioUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.PortfolioUsers.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, PortfolioUser user)
        {
            try
            {
                if (id != user.Id)
                {
                    return BadRequest("User ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _context.PortfolioUsers.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                existingUser.Name = user.Name;
                existingUser.Bio = user.Bio;
                existingUser.ProfileImageUrl = user.ProfileImageUrl;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.PortfolioUsers.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                _context.PortfolioUsers.Remove(user);
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
