using System.ComponentModel.DataAnnotations;

namespace EventManager.Server.ApiModels.EventController
{
    public class EventCreateDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Location is required.")]
        [StringLength(100)]
        public required string Location { get; set; }

        public string? Country { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int? Capacity { get; set; }
    }
}
