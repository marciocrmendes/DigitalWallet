using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; } = [];
    }
}
