using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    /// <summary>
    /// This table is used for keeping track of likes on comments
    /// </summary>
    public partial class LikedComment
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CommentId { get; set; }

        public virtual Comment Comment { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
