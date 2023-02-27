using Microsoft.AspNetCore.Authorization;
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
                comments = commentsNumber,
                likes = likes,
            };

            return profile;
        }

        // GET: api/Profile/id/{userId}
        [HttpGet("username/{username}")]
        public async Task<ActionResult<ProfileDTO>> GetProfileUSername(string username)
        {
            Console.WriteLine("Fetching profile");
            var user = await _context.Users.Where(user => user.UserName == username).FirstOrDefaultAsync();
            var commentsNumber = _context.Comments.Where(comment => comment.UserId == user.Id).Count();
            var comments = _context.Comments.Where(comment => comment.UserId == user.Id).ToList();
            long likes = 0;
            comments.ForEach(comment =>
            {
                likes += comment.Likes;
            });

            ProfileDTO profile = new ProfileDTO
            {
                userId = user.Id,
                username = user.UserName,
                comments = commentsNumber,
                likes = likes,
            };

            return profile;
        }

        // GET: api/Profile/avatar/{userId}
        [HttpGet("avatar/id/{userId}")]
        public async Task<ActionResult<ProfileDTO>> GetAvatar(string userId)
        {
            var profile = _context.Profile.Where(prof => prof.UserId == userId).FirstOrDefault();

            if (profile == null) return NotFound();

            return PhysicalFile(profile.Image, "image/jpeg");
        }

        // GET: api/Profile/avatar/{userId}
        [HttpGet("avatar/username/{username}")]
        public async Task<ActionResult<ProfileDTO>> GetAvatarUsername(string username)
        {
            var user = await _context.Users.Where(user => user.UserName == username).FirstOrDefaultAsync();
            var profile = _context.Profile.Where(prof => prof.UserId == user.Id).FirstOrDefault();

            if (profile == null) return NotFound();

            return PhysicalFile(profile.Image, "image/jpeg");
        }

        // GET: api/Profile/banner/{userId}
        [HttpGet("banner/id/{userId}")]
        public async Task<ActionResult<ProfileDTO>> GetBanner(string userId)
        {
            var profile = _context.Profile.Where(prof => prof.UserId == userId).FirstOrDefault();

            if (profile == null) return NotFound();

            return PhysicalFile(profile.Banner, "image/jpeg");
        }

        // GET: api/Profile/banner/{userId}
        [HttpGet("banner/username/{username}")]
        public async Task<ActionResult<ProfileDTO>> GetBannerUsername(string username)
        {
            var user = await _context.Users.Where(user => user.UserName == username).FirstOrDefaultAsync();
            var profile = _context.Profile.Where(prof => prof.UserId == user.Id).FirstOrDefault();

            if (profile == null) return NotFound();

            return PhysicalFile(profile.Banner, "image/jpeg");
        }

        // GET api/<ProfileController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProfileController>
        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] AvatarPostBody avatar)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);

            if (identityName == null) return NotFound();
            string userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            Console.WriteLine("Name: {0} \nImage: {1}", avatar.name, avatar.image);
            Console.WriteLine(_webHostEnvironment.ContentRootPath);
            Guid id = Guid.NewGuid();
            var profilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", id.ToString() + ".jpg");

            var profile = await _context.Profile.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if (profile == null)
            {
                var newProfile = new Profile();
                newProfile.UserId = userId;
                newProfile.CreatedAt = DateTime.Now;
                newProfile.UpdatedAt = DateTime.Now;
                newProfile.Image = profilePath;

                _context.Profile.Add(newProfile);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (ProfileExists(newProfile.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            } else
            {
                if (System.IO.File.Exists(profile.Image)) System.IO.File.Delete(profile.Image);
                profile.Image = profilePath;
                profile.UpdatedAt = DateTime.Now;

                _context.Entry(profile).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            using (var fileStream = new FileStream(profilePath, FileMode.Create))
            {
                avatar.image.CopyTo(fileStream);
            }

            return NoContent();
        }

        // POST api/<ProfileController>
        [Authorize]
        [HttpPost("banner")]
        public async Task<IActionResult> UploadBanner([FromForm] AvatarPostBody banner)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);

            if (identityName == null) return NotFound();
            string userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            Console.WriteLine("Name: {0} \nImage: {1}", banner.name, banner.image);
            Console.WriteLine(_webHostEnvironment.ContentRootPath);
            Guid id = Guid.NewGuid();
            var profilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", id.ToString() + ".jpg");

            var profile = await _context.Profile.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if (profile == null)
            {
                var newProfile = new Profile();
                newProfile.UserId = userId;
                newProfile.CreatedAt = DateTime.Now;
                newProfile.UpdatedAt = DateTime.Now;
                newProfile.Banner = profilePath;

                _context.Profile.Add(newProfile);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (ProfileExists(newProfile.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(profile.Banner)) System.IO.File.Delete(profile.Banner);
                profile.Banner = profilePath;
                profile.UpdatedAt = DateTime.Now;

                _context.Entry(profile).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            using (var fileStream = new FileStream(profilePath, FileMode.Create))
            {
                banner.image.CopyTo(fileStream);
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

        private bool ProfileExists(long id)
        {
            return _context.Profile.Any(e => e.Id == id);
        }
    }
}
