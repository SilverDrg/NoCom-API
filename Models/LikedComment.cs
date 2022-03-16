using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NoCom_API.Models
{
    /// <summary>
    /// This table is used for keeping track of likes on comments
    /// </summary>
    [Table("liked_comments", Schema = "nocom")]
    public partial class LikedComment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("comment_id")]
        public int CommentId { get; set; }

        [ForeignKey("CommentId")]
        [InverseProperty("LikedComments")]
        public virtual Comment Comment { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("LikedComments")]
        public virtual User User { get; set; } = null!;
    }
}
