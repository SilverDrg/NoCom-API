#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCom_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace NoCom_API.Controllers
{
    public class CommentDTO
    {
        public long Id { get; set; }
        public string Content { get; set; } = null!;
        public string EncryptedUrl { get; set; }
        public long Likes { get; set; }
        public bool Nsfw { get; set; }
        public long? ReplyTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Username { get; set; }

        public virtual CommentDTO? ReplyToNavigation { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly NoComContext _context;

        public CommentsController(NoComContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await _context.Comments.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{hash}/{page}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetCommentsPage(string hash, int page)
        {
            var urlHash = GetHashString(hash);
            var comment = await _context.Comments
                //.Where(comment => comment.Website.UrlHash == urlHash)
                .Select(comment => new CommentDTO 
                { 
                    Id = comment.Id,
                    Content = comment.Content,
                    EncryptedUrl = comment.EncryptedUrl,
                    Likes = comment.Likes,
                    Nsfw = comment.Nsfw,
                    ReplyTo = comment.ReplyTo,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                    Username = comment.User.UserName,
                })
                .Skip((page-1)*20)
                .Take(20)
                .ToListAsync();

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // GET: api/Comment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(long id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(long id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CommentPostBody commentData)
        {
            Console.WriteLine(
                "UserId: {0} \nContent: {1} \nWebsite: {2} \nNsfw: {3}", 
                commentData.userId, 
                commentData.content, 
                commentData.website, 
                commentData.nsfw
            );

            var urlHash = GetHashString(commentData.website);

            if (!WebsiteExists(urlHash))
            {
                var newWebsite = new Website();
                newWebsite.UrlHash = urlHash;
                _context.Websites.Add(newWebsite);
                try
                {
                    await _context.SaveChangesAsync(true);
                }
                catch (DbUpdateException e)
                {
                    Console.WriteLine(e);
                }
            }

            var website = _context.Websites.Where(e => e.UrlHash == urlHash).FirstOrDefault();
            var comment = new Comment();
            comment.UserId = commentData.userId;
            comment.Content = commentData.content;
            comment.Website = website;
            comment.Likes = 0;
            comment.Nsfw = commentData.nsfw;
            comment.CreatedAt = DateTime.Now;
            comment.UpdatedAt = DateTime.Now;

            _context.Comments.Add(comment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CommentExists(comment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(long id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

        private bool WebsiteExists(string websiteHash)
        {
            return _context.Websites.Any(e => e.UrlHash == websiteHash);
        }

        private static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            var sb = new System.Text.StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public class CommentPostBody
        {
            public string userId { get; set; }
            public string content { get; set; }
            public string website { get; set; }
            public bool nsfw { get; set; }

        }
    }
}
