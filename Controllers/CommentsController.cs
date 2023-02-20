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
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using NoCom_API.Tools;

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
        public bool IsDeleted { get; set; } = false;
        public bool IsLiked { get; set; } = false;
        public string Username { get; set; }
        public Boolean IsOwner { get; set; } = false;
        public long RepliesCount { get; set; } = 0;

        public List<CommentDTO>? Replies { get; set; } = new List<CommentDTO>();
    }

    public class CommentPostBody
    {
        public string userId { get; set; }
        public string content { get; set; }
        public string website { get; set; }
        public bool nsfw { get; set; }
        public string replyId { get; set; } = null!;

    }

    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly NoComContext _context;
        private static int _pageSize = 10;

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

        // GET: api/Comments/all/{website}/{orderBy}/{page}
        [HttpGet("all/{hash}/{orderBy}/{page}/{nsfw}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetCommentsPage(string hash, string orderBy, int page, bool nsfw)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);
            string userId = null;
            if (identityName != null) userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            var urlHash = GetHashString(hash);
            var query = _context.Comments
                //.Where(comment => comment.Website.UrlHash == urlHash && comment.IsDeleted == false && comment.Nsfw == nsfw)
                .Include(comment => comment.User)
                .Include(comment => comment.InverseReplyToNavigation);
            var comments = await Helper.OrderBy(query, OrderByParameter(orderBy), false)
                .Skip((page - 1)* _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var commentsDto = new List<CommentDTO>();
            comments.ForEach(comment => commentsDto.Add(ModelToDTOWithVirtual(_context, comment, userId)));

            if (commentsDto == null && commentsDto.Count <= 0)
            {
                return NotFound();
            }

            return commentsDto;
        }

        // GET: api/Comments/user/{sorting}/{page}
        [HttpGet("user/{orderBy}/{page}/{nsfw}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetUserCommentsTopPage(string orderBy, int page, bool nsfw)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);

            if (identityName == null) return NotFound();
            string userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            var query = _context.Comments
                .Where(comment => comment.IsDeleted == false && comment.UserId == userId && (nsfw || comment.Nsfw == nsfw))
                .Include(comment => comment.User)
                .Include(comment => comment.InverseReplyToNavigation)
                .Include(comment => comment.Website);

            var comments = await Helper.OrderBy(query, OrderByParameter(orderBy), false)
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var commentsDto = new List<CommentDTO>();
            comments.ForEach(comment => commentsDto.Add(ModelToDTOWithVirtual(_context, comment, userId)));

            if (commentsDto == null && commentsDto.Count <= 0)
            {
                return NotFound();
            }

            return commentsDto;
        }

        // GET: api/Comments/{website}/{id}
        [HttpGet("{website}/{id}")]
        public async Task<ActionResult<Comment>> GetComment(string website, long id)
        {
            var urlHash = GetHashString(website);
            var comment = await _context.Comments
                .Where(comment => comment.Website.UrlHash == urlHash && comment.Id == id)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // GET: api/Comment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDTO>> GetComment(long id)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);
            string userId = null;
            if (identityName != null) userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            var comment = await _context.Comments
                .Include(comment => comment.User)
                .Include(comment => comment.InverseReplyToNavigation)
                .Where(comment => comment.Id == id)
                .FirstOrDefaultAsync();

            var commentDTO = ModelToDTO(_context, comment, userId);

            if (comment == null)
            {
                return NotFound();
            }

            return commentDTO;
        }

        // GET: api/Comment/{id}
        [HttpGet("{id}/{orderBy}/{page}/{nsfw}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetReplies(long id, string orderBy, int page, bool nsfw)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);
            string userId = null;
            if (identityName != null) userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            var query = _context.Comments
                .Include(comment => comment.User)
                .Include(comment => comment.InverseReplyToNavigation)
                .Where(comment => comment.ReplyTo == id);

            var replies = await Helper.OrderBy(query, OrderByParameter(orderBy), false)
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();
            Console.WriteLine("Replies found: {0}", replies.Count);

            var commentDTO = new List<CommentDTO>();
            replies.ForEach(reply => commentDTO.Add(ModelToDTO(_context, reply, userId)));

            if (replies == null)
            {
                return NotFound();
            }

            return commentDTO;
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
                "UserId: {0} \nContent: {1} \nWebsite: {2} \nNsfw: {3} \nReply to: {4}", 
                commentData.userId, 
                commentData.content, 
                commentData.website, 
                commentData.nsfw,
                commentData.replyId
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

            if (commentData.replyId != "undefined") comment.ReplyTo = Convert.ToInt64(commentData.replyId);

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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);

            if (identityName == null) return NotFound();
            string userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId == userId)
            {
                comment.IsDeleted = true;
                comment.UpdatedAt = DateTime.Now;
                _context.Entry(comment).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        public static CommentDTO ModelToDTO(NoComContext _context, Comment comment, string? userId)
        {
            var isOwner = false;
            var isLiked = false;
            var commentContent = comment.Content;
            var username = comment.User?.UserName;

            if (userId != null)
            {
                isOwner = comment.UserId == userId;
                isLiked = IsLiked(_context, comment, userId);
            }

            if (comment.IsDeleted)
            {
                commentContent = "This comment is no longer available";
                username = null;
            }

            var dto = new CommentDTO
            {
                Id = comment.Id,
                Content = commentContent,
                EncryptedUrl = comment?.EncryptedUrl,
                Likes = comment.Likes,
                Nsfw = comment.Nsfw,
                ReplyTo = comment?.ReplyTo,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                IsDeleted = comment.IsDeleted,
                Username = username,
                IsOwner = isOwner,
                IsLiked = isLiked,
                RepliesCount = comment.InverseReplyToNavigation.Count(),
            };

            return dto;
        }

        public static CommentDTO ModelToDTOWithVirtual(NoComContext _context, Comment comment, string? userId)
        {
            CommentDTO dto = ModelToDTO(_context, comment, userId);
            List<CommentDTO> dtoReplies = new List<CommentDTO>();
            var replies = comment.InverseReplyToNavigation.OrderByDescending(c => c.CreatedAt).Take(5).ToList();
            foreach (var (reply, index) in replies.Select((value, i) => (value, i)))
            {
                if (index == 5) break;
                dtoReplies.Add(ModelToDTO(_context, reply, userId));
            }
            dto.Replies = dtoReplies;

            return dto;
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

        private static bool IsLiked(NoComContext _context, Comment comment, string userId)
        {
            var likedComment = _context.LikedComments
                .Where(e => 
                    e.UserId == userId && 
                    e.CommentId == comment.Id)
                .FirstOrDefault();

            return likedComment != null;
        }

        private static string OrderByParameter(string sorting)
        {
            return sorting switch
            {
                "new" => "CreatedAt",
                "top" => "Likes",
                _ => "CreatedAt",
            };
        }
    }
}
