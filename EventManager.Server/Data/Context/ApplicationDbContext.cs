using EventManager.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Server.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger)
            : base(options)
        {
            _logger = logger;
        }

        public virtual DbSet<Event> Events { get; set; } = null!;
    }
}
