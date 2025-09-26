using System.ComponentModel.DataAnnotations;
using EventManager.Server.ValidationAttributes;

namespace EventManager.Server.ApiModels.Authorization
{
    public class LoginDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = $"{nameof(Email)} is required.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Password]
        public required string Password { get; set; }
    }
}
