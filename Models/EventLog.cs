using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    /// <summary>
    /// Save events such as log in, log out and others
    /// </summary>
    public partial class EventLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TimeOnly EventTime { get; set; }
        public string Event { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
