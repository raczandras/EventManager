using EventManager.Server.ApiModels.Common;
using EventManager.Server.Data.Context;
using EventManager.Server.Data.Entities;
using EventManager.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Repositories
{
    public class EventRepositoryTests
    {
        private ApplicationDbContext _context;
        private EventRepository _repository;
        private List<Event> _events;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var logger = Mock.Of<ILogger<ApplicationDbContext>>();
            _context = new ApplicationDbContext(options, logger);

            _events = new List<Event>
            {
                new() { EventId = 1, Name = "Event 1", Location = "Location 1" },
                new() { EventId = 2, Name = "Event 2", Location = "Location 2" },
                new() { EventId = 3, Name = "Event 3", Location = "Location 3" }
            };

            _context.Events.AddRange(_events);
            _context.SaveChanges();

            _repository = new EventRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetEventByIdAsync_WhenEventExists_ReturnsEvent()
        {
            // Arrange
            ulong eventId = 1;
            var expectedEvent = _events.First();

            // Act
            var result = await _repository.GetEventByIdAsync(eventId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.EventId, Is.EqualTo(eventId));
                Assert.That(result.Name, Is.EqualTo(expectedEvent.Name));
                Assert.That(result.Location, Is.EqualTo(expectedEvent.Location));
            });
        }

        [Test]
        public async Task GetEventsAsync_WithDefaultQuery_ReturnsAllEvents()
        {
            // Arrange
            var query = new PaginationQuery();

            // Act
            var result = await _repository.GetEventsAsync(query);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Items, Has.Count.EqualTo(_events.Count));
                Assert.That(result.TotalCount, Is.EqualTo(_events.Count));
                Assert.That(result.Page, Is.EqualTo(1));
                Assert.That(result.PageSize, Is.EqualTo(_events.Count));
            });
        }

        [Test]
        public async Task GetEventsAsync_WithPagination_ReturnsPagedEvents()
        {
            // Arrange
            var query = new PaginationQuery { Page = 2, PageSize = 1 };

            // Act
            var result = await _repository.GetEventsAsync(query);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Items, Has.Count.EqualTo(1));
                Assert.That(result.TotalCount, Is.EqualTo(_events.Count));
                Assert.That(result.Items.First().EventId, Is.EqualTo(2));
                Assert.That(result.Page, Is.EqualTo(2));
                Assert.That(result.PageSize, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetEventsAsync_WithSorting_ReturnsSortedEvents()
        {
            // Arrange
            var query = new PaginationQuery { SortBy = "Name", Descending = true };

            // Act
            var result = await _repository.GetEventsAsync(query);

            // Assert
            var items = result.Items.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(items, Has.Count.EqualTo(_events.Count));
                Assert.That(items[0].Name, Is.EqualTo("Event 3"));
                Assert.That(items[1].Name, Is.EqualTo("Event 2"));
                Assert.That(items[2].Name, Is.EqualTo("Event 1"));
            });
        }

        [Test]
        public async Task CreateEventAsync_WithValidEvent_ReturnsCreatedEvent()
        {
            // Arrange
            var newEvent = new Event { Name = "New Event", Location = "New Location" };

            // Act
            var result = await _repository.CreateEventAsync(newEvent);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.EqualTo(newEvent.Name));
                Assert.That(result.Location, Is.EqualTo(newEvent.Location));
            });

            var savedEvent = await _context.Events.FindAsync(result.EventId);
            Assert.That(savedEvent, Is.Not.Null);
        }

        [Test]
        public async Task UpdateEventAsync_WithValidEvent_ReturnsUpdatedEvent()
        {
            // Arrange
            var eventToUpdate = await _context.Events.FindAsync((ulong)1);
            Assert.That(eventToUpdate, Is.Not.Null);
            eventToUpdate.Name = "Updated Event";

            // Act
            var result = await _repository.UpdateEventAsync(eventToUpdate);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.EqualTo("Updated Event"));
            });

            var savedEvent = await _context.Events.FindAsync((ulong)1);
            Assert.That(savedEvent?.Name, Is.EqualTo("Updated Event"));
        }

        [Test]
        public async Task DeleteEventAsync_WithExistingEvent_ReturnsTrue()
        {
            // Arrange
            var eventToDelete = await _context.Events.FindAsync((ulong)1);
            Assert.That(eventToDelete, Is.Not.Null);

            // Act
            var result = await _repository.DeleteEventAsync(eventToDelete);

            // Assert
            Assert.That(result, Is.True);
            var deletedEvent = await _context.Events.FindAsync((ulong)1);
            Assert.That(deletedEvent, Is.Null);
        }
    }
}