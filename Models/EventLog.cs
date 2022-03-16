using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NoCom_API.Models
{
    /// <summary>
    /// Save events such as log in, log out and others
    /// </summary>
    [Table("event_logs", Schema = "nocom")]
    public partial class EventLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("event_time")]
        public TimeSpan EventTime { get; set; }
        [Column("event")]
        public string Event { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("EventLogs")]
        public virtual User User { get; set; } = null!;
    }
}
