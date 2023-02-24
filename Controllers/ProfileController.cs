using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCom_API.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoCom_API.Controllers
{
    public class ProfileDTO
    {
        public string userId { get; set; }
        public string username { get; set; }
        public string avatar { get; set; }
        public int comments { get; set; }
        public long likes { get; set; }
    }

    public class AvatarPostBody
    {
        public string name { get; set; }
        public IFormFile image { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly NoComContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(NoComContext context, IWebHostEnvironment environment)
        {
            _webHostEnvironment = environment;
            _context = context;
        }
        // GET: api/Profile/id/{userId}
        [HttpGet("id/{userId}")]
        public async Task<ActionResult<ProfileDTO>> GetProfile(string userId)
        {
            Console.WriteLine("Fetching profile");
            var user = await _context.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
            var commentsNumber = _context.Comments.Where(comment => comment.UserId == userId).Count();
            var comments = _context.Comments.Where(comment => comment.UserId == userId).ToList();
            long likes = 0;
            comments.ForEach(comment =>
            {
                likes += comment.Likes;
            });

            ProfileDTO profile = new ProfileDTO
            {
                userId = user.Id,
                username = user.UserName,
                avatar = "",
                comments = commentsNumber,
                likes = likes,
            };

            return profile;
        }

        // GET api/<ProfileController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProfileController>
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] AvatarPostBody avatar)
        {
            Console.WriteLine("Name: {0} \nImage: {1}", avatar.name, avatar.image);
            Console.WriteLine(_webHostEnvironment.ContentRootPath);
            Guid id = Guid.NewGuid();
            using (var fileStream = new FileStream(Path.Combine(_webHostEnvironment.ContentRootPath, "Images", id.ToString() + ".jpg"), FileMode.Create))
            {
                avatar.image.CopyTo(fileStream);
            }
            return NoContent();
        }

        // PUT api/<ProfileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProfileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
