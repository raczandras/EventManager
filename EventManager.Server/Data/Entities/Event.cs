using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManager.Server.Data.Entities
{
    [Table("Events")]
    public class Event
    {
        [Key]
        public ulong EventId { get; set; }

        public required string Name { get; set; }

        [StringLength(100)]
        public required string Location { get; set; }

        public string? Country { get; set; }

        public uint? Capacity { get; set; }
    }
}
