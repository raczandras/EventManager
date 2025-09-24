using EventManager.Server.ApiModels.Common;
using EventManager.Server.Data.Entities;

namespace EventManager.Server.Interfaces
{
    public interface IEventRepository
    {
        public Task<Event?> GetEventByIdAsync(int id);
        public Task<PagedResult<Event>> GetEventsAsync(PaginationQuery query);
        public Task<Event> CreateEventAsync(Event newEvent);
        public Task<Event> UpdateEventAsync(Event updatedEvent);
        public Task<bool> DeleteEventAsync(Event entity);
    }
}
