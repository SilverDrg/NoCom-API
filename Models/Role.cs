using System;
using System.Collections.Generic;

namespace NoCom_API.Models
{
    public partial class Role
    {
        public Role()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        public long Id { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
