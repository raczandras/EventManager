using EventManager.Server.ApiModels.Common;
using EventManager.Server.Data.Context;
using EventManager.Server.Data.Entities;
using EventManager.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Server.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EventRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Event?> GetEventByIdAsync(ulong eventId)
        {
            return await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(entity => entity.EventId == eventId);
        }

        public async Task<PagedResult<Event>> GetEventsAsync(PaginationQuery query)
        {
            var queryable = _dbContext.Events.AsQueryable();

            //Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                queryable = query.Descending
                    ? queryable.OrderByDescending(entity => EF.Property<object>(entity, query.SortBy))
                    : queryable.OrderBy(entity => EF.Property<object>(entity, query.SortBy));
            }
            else
            {
                queryable = queryable.OrderBy(e => e.EventId); //default
            }

            var totalCount = await queryable.CountAsync();

            //Pagination
            if (query.Page.HasValue && query.PageSize.HasValue)
            {
                queryable = queryable.Skip(((query.Page.Value - 1) * query.PageSize.Value))
                     .Take(query.PageSize.Value);
            }

            var items = await queryable.ToListAsync();

            return new PagedResult<Event>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page ?? 1,
                PageSize = query.PageSize ?? totalCount
            };
        }

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            var entity = await _dbContext.Events.AddAsync(newEvent);
            await _dbContext.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<Event> UpdateEventAsync(Event updatedEvent)
        {
            var entity = _dbContext.Events.Update(updatedEvent);
            await _dbContext.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<bool> DeleteEventAsync(Event entity)
        {
            _dbContext.Events.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
