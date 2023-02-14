#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCom_API.Models;

namespace NoCom_API.Controllers
{
    public class LikePostBody
    {
        public bool liked { get; set; }
        public long commentId { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LikedCommentsController : ControllerBase
    {
        private readonly NoComContext _context;
        private static int _pageSize = 10;

        public LikedCommentsController(NoComContext context)
        {
            _context = context;
        }

        // GET: api/LikedComments/user/{page}
        [Authorize]
        [HttpGet("user/{sortBy}/{page}/{nsfw}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetLikedComments(string sortBy, int page, bool nsfw)
        {
            string identityName = HttpContext.User.Identity.Name;
            Console.WriteLine("User Name: {0} ", identityName);

            if (identityName == null) return NotFound();
            string userId = (await _context.Users.Where(x => x.UserName == identityName).FirstOrDefaultAsync()).Id;

            var comments = await _context.LikedComments
                .Include(x => x.Comment.User)
                .Include(x => x.Comment.Website)
                .Include(x => x.Comment.InverseReplyToNavigation)
                .Where(x => x.UserId == userId && x.Comment.IsDeleted == false && (nsfw || x.Comment.Nsfw == nsfw))
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();


            var commentsDto = new List<CommentDTO>();
            comments.ForEach(comment => commentsDto.Add(CommentsController.ModelToDTOWithVirtual(_context, comment.Comment, userId)));

            if (commentsDto == null && commentsDto.Count <= 0)
            {
                return NotFound();
            }

            return commentsDto;
        }

        // GET: api/LikedComments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LikedComment>> GetLikedComment(long id)
        {
            var likedComment = await _context.LikedComments.FindAsync(id);

            if (likedComment == null)
            {
                return NotFound();
            }

            return likedComment;
        }

        // PUT: api/LikedComments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLikedComment(long id, LikedComment likedComment)
        {
            if (id != likedComment.Id)
            {
                return BadRequest();
            }

            _context.Entry(likedComment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikedCommentExists(id))
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

        // POST: api/LikedComments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("{commentId}")]
        public async Task<ActionResult<int>> PostLikedComment(LikePostBody likeBody)
        {
            string identityName = HttpContext.User.Identity.Name;
            var user = _context.Users.Where(user => user.UserName == identityName).FirstOrDefault();
            if (user == null) return NotFound();
            if (likeBody.liked)
            {
                Console.WriteLine("Adding like");
                var likedComment = new LikedComment
                {
                    CommentId = likeBody.commentId,
                    UserId = user.Id,
                };

                var exists = _context.LikedComments.Where(comment => comment.UserId == user.Id && comment.CommentId == likeBody.commentId).FirstOrDefault();
                if (exists != null) return NoContent();

                _context.LikedComments.Add(likedComment);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (LikedCommentExists(likedComment.Id))
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
                Console.WriteLine("Removing like");
                await DeleteLike(likeBody.commentId);
            }

            var currentLikes = _context.LikedComments.Where(like => like.CommentId == likeBody.commentId).Count();
            var comment = _context.Comments.Where(comment => comment.Id == likeBody.commentId).FirstOrDefault();
            comment.Likes = currentLikes;
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            Console.WriteLine("Like updated, current count is {0}", currentLikes);

            return currentLikes;
        }

        // DELETE: api/LikedComments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLikedComment(long id)
        {
            var likedComment = await _context.LikedComments.FindAsync(id);
            if (likedComment == null)
            {
                return NotFound();
            }

            _context.LikedComments.Remove(likedComment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<IActionResult> DeleteLike(long id)
        {
            var likedComment = await _context.LikedComments.FirstOrDefaultAsync(like => like.CommentId == id);
            if (likedComment == null)
            {
                return NotFound();
            }

            _context.LikedComments.Remove(likedComment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LikedCommentExists(long id)
        {
            return _context.LikedComments.Any(e => e.Id == id);
        }
    }
}
