using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NoCom_API.Models
{
    /// <summary>
    /// Comments from users
    /// </summary>
    [Table("comments", Schema = "nocom")]
    public partial class Comment
    {
        public Comment()
        {
            LikedComments = new HashSet<LikedComment>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("website_id")]
        public int WebsiteId { get; set; }
        [Column("comment")]
        public string Comment1 { get; set; } = null!;
        [Column("likes")]
        public int Likes { get; set; }
        [Column("nsfw")]
        public bool Nsfw { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Comments")]
        public virtual User User { get; set; } = null!;
        [ForeignKey("WebsiteId")]
        [InverseProperty("Comments")]
        public virtual Website Website { get; set; } = null!;
        [InverseProperty("Comment")]
        public virtual ICollection<LikedComment> LikedComments { get; set; }
    }
}
