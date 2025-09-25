using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManager.Server.Data.Entities
{
    [Table("Events")]
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        public required string Name { get; set; }

        [StringLength(100)]
        public required string Location { get; set; }

        public string? Country { get; set; }

        public int? Capacity { get; set; }
    }
}
