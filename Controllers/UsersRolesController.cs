#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCom_API.Models;

namespace NoCom_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersRolesController : ControllerBase
    {
        private readonly NoComContext _context;

        public UsersRolesController(NoComContext context)
        {
            _context = context;
        }

        // GET: api/UsersRoles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersRole>>> GetUsersRoles()
        {
            return await _context.UsersRoles.ToListAsync();
        }

        // GET: api/UsersRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsersRole>> GetUsersRole(long id)
        {
            var usersRole = await _context.UsersRoles.FindAsync(id);

            if (usersRole == null)
            {
                return NotFound();
            }

            return usersRole;
        }

        // PUT: api/UsersRoles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsersRole(long id, UsersRole usersRole)
        {
            if (id != usersRole.Id)
            {
                return BadRequest();
            }

            _context.Entry(usersRole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersRoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UsersRoles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UsersRole>> PostUsersRole(UsersRole usersRole)
        {
            _context.UsersRoles.Add(usersRole);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UsersRoleExists(usersRole.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUsersRole", new { id = usersRole.Id }, usersRole);
        }

        // DELETE: api/UsersRoles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsersRole(long id)
        {
            var usersRole = await _context.UsersRoles.FindAsync(id);
            if (usersRole == null)
            {
                return NotFound();
            }

            _context.UsersRoles.Remove(usersRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersRoleExists(long id)
        {
            return _context.UsersRoles.Any(e => e.Id == id);
        }
    }
}
