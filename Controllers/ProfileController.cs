using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCom_API.Models;

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

    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly NoComContext _context;
        private static int _pageSize = 10;

        public ProfileController(NoComContext context)
        {
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
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
