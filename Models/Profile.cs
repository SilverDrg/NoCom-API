using Microsoft.AspNetCore.Identity;

namespace NoCom_API.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public byte[]? Image { get; set; }
        public byte[]? Banner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual IdentityUser User { get; set; } = null!;
    }
}
