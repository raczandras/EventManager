using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventManager.Server.Data.Entities
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public required string UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
