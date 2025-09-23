using EventManager.Server.ApiModels.Common;
using EventManager.Server.ApiModels.EventController;

namespace EventManager.Server.Interfaces
{
    public interface IEventService
    {
        public Task<EventDto?> GetEventByIdAsync(ulong id);
        public Task<PagedResult<EventDto>> GetEventsAsync(PaginationQuery query);
        public Task<EventDto> CreateEventAsync(EventCreateDto dto);
        public Task<EventDto?> UpdateEventAsync(EventDto dto);
        public Task<bool> DeleteEventAsync(ulong id);
    }
}
