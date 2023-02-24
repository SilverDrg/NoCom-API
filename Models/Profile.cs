using Microsoft.AspNetCore.Identity;

namespace NoCom_API.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? Image { get; set; }
        public string? Banner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual IdentityUser User { get; set; } = null!;
    }
}
