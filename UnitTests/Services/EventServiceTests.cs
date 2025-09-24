using AutoMapper;
using EventManager.Server.ApiModels.Common;
using EventManager.Server.ApiModels.EventController;
using EventManager.Server.Data.Entities;
using EventManager.Server.Interfaces;
using EventManager.Server.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Services
{
    public class EventServiceTests
    {
        private Mock<IEventRepository> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<EventService>> _mockLogger;
        private EventService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IEventRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<EventService>>();
            _service = new EventService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetEventByIdAsync_WhenEventExists_ReturnsEventDto()
        {
            // Arrange
            int eventId = 1;
            var eventEntity = new Event { EventId = eventId, Name = "Test Event", Location = "Test Location" };
            var expectedDto = new EventDto { EventId = eventId, Name = "Test Event", Location = "Test Location" };

            _mockRepository.Setup(x => x.GetEventByIdAsync(eventId)).ReturnsAsync(eventEntity);
            _mockMapper.Setup(x => x.Map<Event?, EventDto?>(eventEntity)).Returns(expectedDto);

            // Act
            var result = await _service.GetEventByIdAsync(eventId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(expectedDto));
            });
        }

        [Test]
        public async Task GetEventByIdAsync_WhenEventDoesNotExist_ReturnsNull()
        {
            // Arrange
            int eventId = 1;
            _mockRepository.Setup(x => x.GetEventByIdAsync(eventId)).ReturnsAsync((Event?)null);
            _mockMapper.Setup(x => x.Map<Event?, EventDto?>(null)).Returns((EventDto?)null);

            // Act
            var result = await _service.GetEventByIdAsync(eventId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEventsAsync_WithValidQuery_ReturnsPagedResult()
        {
            // Arrange
            var query = new PaginationQuery { Page = 1, PageSize = 10 };
            var eventEntities = new List<Event>
            {
                new() { EventId = 1, Name = "Test Event", Location = "Test Location" }
            };
            var eventDtos = new List<EventDto>
            {
                new() { EventId = 1, Name = "Test Event", Location = "Test Location" }
            };

            var pagedEntityResult = new PagedResult<Event>
            {
                Items = eventEntities,
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _mockRepository.Setup(x => x.GetEventsAsync(query)).ReturnsAsync(pagedEntityResult);
            _mockMapper.Setup(x => x.Map<List<EventDto>>(eventEntities)).Returns(eventDtos);

            // Act
            var result = await _service.GetEventsAsync(query);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Items, Is.EqualTo(eventDtos));
                Assert.That(result.TotalCount, Is.EqualTo(pagedEntityResult.TotalCount));
                Assert.That(result.Page, Is.EqualTo(pagedEntityResult.Page));
                Assert.That(result.PageSize, Is.EqualTo(pagedEntityResult.PageSize));
            });
        }

        [Test]
        public async Task CreateEventAsync_WithValidData_ReturnsCreatedEventDto()
        {
            // Arrange
            var createDto = new EventCreateDto { Name = "Test Event", Location = "Test Location" };
            var eventEntity = new Event { Name = "Test Event", Location = "Test Location" };
            var createdEntity = new Event { EventId = 1, Name = "Test Event", Location = "Test Location" };
            var expectedDto = new EventDto { EventId = 1, Name = "Test Event", Location = "Test Location" };

            _mockMapper.Setup(x => x.Map<EventCreateDto, Event>(createDto)).Returns(eventEntity);
            _mockRepository.Setup(x => x.CreateEventAsync(eventEntity)).ReturnsAsync(createdEntity);
            _mockMapper.Setup(x => x.Map<Event, EventDto>(createdEntity)).Returns(expectedDto);

            // Act
            var result = await _service.CreateEventAsync(createDto);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDto));
        }

        [Test]
        public async Task UpdateEventAsync_WhenEventExists_ReturnsUpdatedEventDto()
        {
            // Arrange
            var updateDto = new EventDto { EventId = 1, Name = "Updated Event", Location = "Updated Location" };
            var existingEntity = new Event { EventId = 1, Name = "Test Event", Location = "Test Location" };
            var updatedEntity = new Event { EventId = 1, Name = "Updated Event", Location = "Updated Location" };

            _mockRepository.Setup(x => x.GetEventByIdAsync(updateDto.EventId)).ReturnsAsync(existingEntity);
            _mockMapper.Setup(x => x.Map<EventDto, Event>(updateDto)).Returns(updatedEntity);
            _mockRepository.Setup(x => x.UpdateEventAsync(updatedEntity)).ReturnsAsync(updatedEntity);
            _mockMapper.Setup(x => x.Map<Event, EventDto>(updatedEntity)).Returns(updateDto);

            // Act
            var result = await _service.UpdateEventAsync(updateDto);

            // Assert
            Assert.That(result, Is.EqualTo(updateDto));
        }

        [Test]
        public async Task DeleteEventAsync_WhenEventExists_ReturnsTrue()
        {
            // Arrange
            int eventId = 1;
            var existingEntity = new Event { EventId = 1, Name = "Test Event", Location = "Test Location" };

            _mockRepository.Setup(x => x.GetEventByIdAsync(eventId)).ReturnsAsync(existingEntity);
            _mockRepository.Setup(x => x.DeleteEventAsync(existingEntity)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteEventAsync(eventId);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}