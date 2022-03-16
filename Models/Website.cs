using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NoCom_API.Models
{
    /// <summary>
    /// Saving websites without keeping track what kind of websites the users are browsing on
    /// </summary>
    [Table("websites", Schema = "nocom")]
    public partial class Website
    {
        public Website()
        {
            Comments = new HashSet<Comment>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("url_hash")]
        [StringLength(128)]
        public string UrlHash { get; set; } = null!;

        [InverseProperty("Website")]
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
