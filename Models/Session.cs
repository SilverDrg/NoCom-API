using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    /// <summary>
    /// Keeps track of user sessions and session tokens
    /// </summary>
    public partial class Session
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string SessionToken { get; set; } = null!;
        public DateTime SessionEnd { get; set; }
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenEnd { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
