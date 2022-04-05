using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    public partial class UsersRole
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long WebsiteId { get; set; }
        public long RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Website Website { get; set; } = null!;
    }
}
