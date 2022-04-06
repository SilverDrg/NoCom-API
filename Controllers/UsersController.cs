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
        private readonly NoComContext _context;

        public UsersController(NoComContext context)
        {
            _context = context;
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

        // GET: api/Users/Username
        [HttpGet("Username/{id:alpha}")]
        public async Task<IActionResult> GetUsername(string id)
        {
            Console.WriteLine("name: {0}", id);
            var user = await _context.Users.Where(user => user.Username == id).FirstOrDefaultAsync();
            if (user == null)
            {
                Console.WriteLine("Available");
                return Ok("available");
            }
            else
            {
                Console.WriteLine("Taken");
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

        // POST: api/Users/Register
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(User user)
        {
            Console.WriteLine("Received the register data");

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
            return Ok();
        }

        // POST: api/Users/Login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("/Login")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            var existingUser = await _context.Users.Where(e => e.Username == user.Username).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return StatusCode(400, "Username doesn't exists.");
            }

            if (!MatchingPasswords(existingUser.Password, user.Password))
            {
                return StatusCode(400, "Incorrect password");
            }

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

        private bool UsernameExists(string username)
        {
            return _context.Users.Any(e => e.Username == username);
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

        //private string HashPassword(string password)
        //{
        //    var passwordBytes = Encoding.UTF8.GetBytes(password);
        //    var argon2 = new Argon2i(passwordBytes);
        //    var salt = new byte[16];

        //    var random = RandomNumberGenerator.Create();
        //    random.GetBytes(salt);

        //    argon2.DegreeOfParallelism = 2;
        //    argon2.MemorySize = 512;
        //    argon2.Iterations = 4;
        //    argon2.Salt = salt;

        //    var hashedPassword = argon2.GetBytes(112);
        //    var combination = salt.Concat(hashedPassword);
        //    return Encoding.UTF8.GetString(combination.ToArray());
        //}

        private bool MatchingPasswords(string storedPassword, string providedPassword)
        {
            var storedPasswordBytes = Encoding.UTF8.GetBytes(storedPassword);
            var providedPasswordBytes = Encoding.UTF8.GetBytes(providedPassword);
            var argon2 = new Argon2i(providedPasswordBytes);
            var salt = storedPasswordBytes.Take(16).ToArray();

            argon2.DegreeOfParallelism = 2;
            argon2.MemorySize = 512;
            argon2.Iterations = 4;
            argon2.Salt = salt;

            var hashedProvidedPassword = argon2.GetBytes(112);
            var encodedPassword = salt.Concat(hashedProvidedPassword);

            return storedPassword.Equals(Encoding.UTF8.GetString(encodedPassword.ToArray()));
        }
    }
}
