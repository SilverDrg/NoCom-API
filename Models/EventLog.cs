using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    /// <summary>
    /// Save events such as log in, log out and others
    /// </summary>
    public partial class EventLog
    {
        public long Id { get; set; }
        public string UserId { get; set; } = null!;
        public TimeOnly EventTime { get; set; }
        public string Event { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;
    }
}
