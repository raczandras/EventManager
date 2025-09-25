using EventManager.Server.ApiModels.Common;
using EventManager.Server.ApiModels.EventController;
using EventManager.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IEventService _eventService;

        public EventController(ILogger<EventController> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        /// <summary>
        /// Get an event by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the event.</param>
        /// <returns>Returns the event if found, otherwise 404 Not Found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _eventService.GetEventByIdAsync(id);

            if (result == null)
            {
                string message = $"Event with ID {id} not found.";

                _logger.LogWarning(message);
                return NotFound(new { Message = message });
            }
            return Ok(result);
        }

        /// <summary>
        /// Get a list of events with optional pagination and sorting.
        /// </summary>
        /// <param name="query">Pagination and sorting parameters.</param>
        /// <returns>Paged list of events.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEvents([FromQuery] PaginationQuery query)
        {
            try
            {
                var result = await _eventService.GetEventsAsync(query);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Bad query request: {Message}", ex.Message);
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Create a new event.
        /// </summary>
        /// <param name="dto">The event details.</param>
        /// <returns>The created event, or 400 if a parameter is bad.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>> CreateEvent(EventCreateDto dto)
        {
            var created = await _eventService.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetEventById), new { id = created.EventId }, created);
        }

        /// <summary>
        /// Update an existing event.
        /// </summary>
        /// <param name="dto">The updated event details.</param>
        /// <returns>The updated event, or 404 if not found, or 400 if a parameter is bad.</returns>
        [HttpPut]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateEvent(EventDto dto)
        {
            var updated = await _eventService.UpdateEventAsync(dto);
            if (updated == null)
            {
                string message = $"Event with ID {dto.EventId} not found.";

                _logger.LogWarning(message);
                return NotFound(new { Message = message });
            }

            return Ok(updated);
        }

        /// <summary>
        /// Delete an event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event.</param>
        /// <returns>No content if deleted, 404 if not found.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            var deleted = await _eventService.DeleteEventAsync(id);
            if (!deleted)
            {
                string message = $"Event with ID {id} not found.";

                _logger.LogWarning(message);
                return NotFound(new { Message = message });
            }

            return NoContent();
        }
    }

}
