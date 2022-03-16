using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NoCom_API.Models
{
    /// <summary>
    /// user information
    /// </summary>
    [Table("users", Schema = "nocom")]
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            EventLogs = new HashSet<EventLog>();
            LikedComments = new HashSet<LikedComment>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        [StringLength(128)]
        public string Username { get; set; } = null!;
        [Column("email")]
        [StringLength(128)]
        public string Email { get; set; } = null!;
        [Column("password")]
        [StringLength(128)]
        public string Password { get; set; } = null!;
        [Column("profile_image")]
        public byte[]? ProfileImage { get; set; }
        [Column("banner_image")]
        public byte[]? BannerImage { get; set; }
        [Column("is_admin")]
        public bool IsAdmin { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Comment> Comments { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<EventLog> EventLogs { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<LikedComment> LikedComments { get; set; }
    }
}
