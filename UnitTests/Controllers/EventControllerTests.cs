using EventManager.Server.ApiModels.Common;
using EventManager.Server.ApiModels.EventController;
using EventManager.Server.Controllers;
using EventManager.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Controllers
{
    public class EventControllerTests
    {
        private Mock<IEventService> _mockEventService;
        private Mock<ILogger<EventController>> _mockLogger;
        private EventController _controller;

        [SetUp]
        public void Setup()
        {
            _mockEventService = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<EventController>>();
            _controller = new EventController(_mockLogger.Object, _mockEventService.Object);
        }

        [Test]
        public async Task GetEventById_WhenEventExists_ReturnsOkResult()
        {
            // Arrange
            int eventId = 1;
            var expectedEvent = new EventDto { EventId = eventId, Name = "Test Event", Location = "Test Location" };
            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId)).ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(expectedEvent));
        }

        [Test]
        public async Task GetEventById_WhenEventDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int eventId = 1;
            _mockEventService.Setup(x => x.GetEventByIdAsync(eventId)).ReturnsAsync((EventDto?)null);

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetEvents_WithValidQuery_ReturnsOkResult()
        {
            // Arrange
            var query = new PaginationQuery { Page = 1, PageSize = 10 };
            var expectedResult = new PagedResult<EventDto>
            {
                Items = new List<EventDto> { new() { EventId = 1, Name = "Test Event", Location = "Test Location" } },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };
            _mockEventService.Setup(x => x.GetEventsAsync(query)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetEvents(query);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GetEvents_WhenArgumentExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var query = new PaginationQuery { Page = 1, PageSize = 10 };
            _mockEventService.Setup(x => x.GetEventsAsync(query)).ThrowsAsync(new ArgumentException("Invalid query"));

            // Act
            var result = await _controller.GetEvents(query);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = (BadRequestObjectResult)result;
            Assert.That(badRequest.Value, Is.Not.Null);
        }

        [Test]
        public async Task CreateEvent_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var createDto = new EventCreateDto { Name = "Test Event", Location = "Test Location" };
            var createdEvent = new EventDto { EventId = 1, Name = createDto.Name, Location = createDto.Location };
            _mockEventService.Setup(x => x.CreateEventAsync(createDto)).ReturnsAsync(createdEvent);

            // Act
            var result = await _controller.CreateEvent(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = (CreatedAtActionResult)result.Result!;
            Assert.Multiple(() =>
            {
                Assert.That(createdResult.ActionName, Is.EqualTo(nameof(EventController.GetEventById)));
                Assert.That(createdResult.RouteValues?["id"], Is.EqualTo(createdEvent.EventId));
                Assert.That(createdResult.Value, Is.EqualTo(createdEvent));
            });
        }

        [Test]
        public async Task UpdateEvent_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var updateDto = new EventDto { EventId = 1, Name = "Updated Event", Location = "Updated Location" };
            _mockEventService.Setup(x => x.UpdateEventAsync(updateDto)).ReturnsAsync(updateDto);

            // Act
            var result = await _controller.UpdateEvent(updateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(updateDto));
        }

        [Test]
        public async Task DeleteEvent_WhenEventExists_ReturnsNoContentResult()
        {
            // Arrange
            int eventId = 1;
            _mockEventService.Setup(x => x.DeleteEventAsync(eventId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteEvent_WhenEntityDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int eventId = 99;
            _mockEventService.Setup(x => x.DeleteEventAsync(eventId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}