using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    /// <summary>
    /// Comments from users
    /// </summary>
    public partial class Comment
    {
        public Comment()
        {
            InverseReplyToNavigation = new HashSet<Comment>();
            LikedComments = new HashSet<LikedComment>();
        }

        public long Id { get; set; }
        public string UserId { get; set; } = null!;
        public long WebsiteId { get; set; }
        public string Content { get; set; } = null!;
        public long Likes { get; set; }
        public bool Nsfw { get; set; }
        public long? ReplyTo { get; set; }

        public virtual Comment? ReplyToNavigation { get; set; }
        public virtual IdentityUser User { get; set; } = null!;
        public virtual Website Website { get; set; } = null!;
        public virtual ICollection<Comment> InverseReplyToNavigation { get; set; }
        public virtual ICollection<LikedComment> LikedComments { get; set; }
    }
}
