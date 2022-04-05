using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    /// <summary>
    /// user information
    /// </summary>
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            EventLogs = new HashSet<EventLog>();
            LikedComments = new HashSet<LikedComment>();
            UsersRoles = new HashSet<UsersRole>();
        }

        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public byte[]? ProfileImage { get; set; }
        public byte[]? BannerImage { get; set; }
        public bool IsAdmin { get; set; }

        public virtual Session IdNavigation { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<EventLog> EventLogs { get; set; }
        public virtual ICollection<LikedComment> LikedComments { get; set; }
        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
