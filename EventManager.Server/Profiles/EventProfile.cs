using AutoMapper;
using EventManager.Server.ApiModels.EventController;
using EventManager.Server.Data.Entities;

namespace EventManager.Server.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<EventCreateDto, Event>();
        }
    }
}
