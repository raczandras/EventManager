using System.ComponentModel.DataAnnotations;

namespace EventManager.Server.ApiModels.Authorization
{
    public class RefreshTokenDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = $"{nameof(RefreshToken)} is required.")]
        public required string RefreshToken { get; set; }
    }
}
