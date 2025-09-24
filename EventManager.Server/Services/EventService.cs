using System.Diagnostics;
using System.Reflection;
using AutoMapper;
using EventManager.Server.ApiModels.Common;
using EventManager.Server.ApiModels.EventController;
using EventManager.Server.Data.Entities;
using EventManager.Server.Interfaces;

namespace EventManager.Server.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository repository, IMapper mapper, ILogger<EventService> logger)
        {
            _eventRepository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();

            var entity = await _eventRepository.GetEventByIdAsync(id);
            var dto = _mapper.Map<Event?, EventDto?>(entity);

            stopwatch.Stop();
            _logger.LogInformation("Fetched event {EventId} in {ElapsedMs} ms", id, stopwatch.ElapsedMilliseconds);

            return dto;
        }

        public async Task<PagedResult<EventDto>> GetEventsAsync(PaginationQuery query)
        {
            //Validate sort property
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var property = typeof(EventDto).GetProperty(
                    query.SortBy,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );

                if (property == null)
                {
                    _logger.LogWarning("Invalid sort property '{SortBy}' requested", query.SortBy);
                    throw new ArgumentException($"Invalid sort property: {query.SortBy}");
                }

                query.SortBy = property.Name; // normalize case
            }

            var stopwatch = Stopwatch.StartNew();

            var result = await _eventRepository.GetEventsAsync(query);

            stopwatch.Stop();

            _logger.LogInformation(
                "Fetched {Count} events (page {Page}, size {PageSize}, sortBy {SortBy}, descending {Descending}) in {ElapsedMs} ms",
                result.Items.Count,
                query.Page ?? 0,
                query.PageSize ?? 0,
                query.SortBy ?? "Id",
                query.Descending,
                stopwatch.ElapsedMilliseconds
            );

            return new PagedResult<EventDto>
            {
                Items = _mapper.Map<List<EventDto>>(result.Items),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<EventDto> CreateEventAsync(EventCreateDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            var entity = _mapper.Map<EventCreateDto, Event>(dto);
            var created = await _eventRepository.CreateEventAsync(entity);
            var result = _mapper.Map<Event, EventDto>(created);

            stopwatch.Stop();
            _logger.LogInformation("Created event {EventId} in {ElapsedMs} ms", created.EventId, stopwatch.ElapsedMilliseconds);

            return result;
        }

        public async Task<EventDto?> UpdateEventAsync(EventDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            var existing = await _eventRepository.GetEventByIdAsync(dto.EventId);
            if (existing == null)
            {
                _logger.LogWarning("Attempted to update event {EventId}, but it was not found", dto.EventId);
                return null;
            }

            var converted = _mapper.Map<EventDto, Event>(dto);
            var updated = await _eventRepository.UpdateEventAsync(converted);
            var result = _mapper.Map<Event, EventDto>(updated);

            stopwatch.Stop();
            _logger.LogInformation("Updated event {EventId} in {ElapsedMs} ms", dto.EventId, stopwatch.ElapsedMilliseconds);

            return result;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();

            var existing = await _eventRepository.GetEventByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Attempted to delete event {EventId}, but it was not found", id);
                return false;
            }

            var deleted = await _eventRepository.DeleteEventAsync(existing);

            stopwatch.Stop();

            _logger.LogInformation("Deleted event {EventId} in {ElapsedMs} ms", id, stopwatch.ElapsedMilliseconds);

            return deleted;
        }
    }
}
