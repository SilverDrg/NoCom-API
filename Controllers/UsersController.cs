#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCom_API.Models;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace NoCom_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public UsersController(CoreDbContext context)
        {
            _context = context;
        }

        private async Task<string> HashPassword(string password)
        {
            return await Task.Run(() =>
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var argon2 = new Argon2i(passwordBytes);
                var salt = new byte[16];

                var random = RandomNumberGenerator.Create();
                random.GetBytes(salt);

                argon2.DegreeOfParallelism = 2;
                argon2.MemorySize = 512;
                argon2.Iterations = 4;
                argon2.Salt = salt;

                var hashedPassword = argon2.GetBytes(112);
                var combination = salt.Concat(hashedPassword);
                return Encoding.UTF8.GetString(combination.ToArray());
            });
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(e => e.Comments).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/username
        [HttpGet("username/{id:alpha}")]
        public async Task<IActionResult> GetUsername(string id)
        {
            Console.WriteLine("name: {0}", id);
            var user = await _context.Users.Where(user => user.Username == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return Ok("available");
            }
            else
            {
                return Ok("taken");
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.Password = await HashPassword(user.Password);

            //var existingUser = await _context.Users.Where(e => e.Username == user.Username).FirstOrDefaultAsync();

            //if (existingUser != null)
            //{
            //    return StatusCode(400, "Username already exists.");
            //}

            user.ProfileImage = null;
            user.BannerImage = null;

            Console.WriteLine("Password length: {0}", user.Password.Length);

            Console.WriteLine("user received: {0} \nid: {1} \nusername: {2} \nemail: {3} \npassword: {4} \nimage: {5} \nbanner: {6} \nisadmin: {7}",
                user, user.Id, user.Username, user.Email, user.Password, user.ProfileImage, user.BannerImage, user.IsAdmin);



            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            //return CreatedAtAction("GetUser", new { id = user.Id }, user);
            return new EmptyResult();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
