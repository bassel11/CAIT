using AutoMapper;
using MeetingApplication.Features.AgendaItems.Queries.Results;
using MeetingCore.Entities;

namespace MeetingApplication.Mappers.AgendaItems
{
    public class AgendaMappingProfile : Profile
    {
        public AgendaMappingProfile()
        {
            CreateMap<AgendaItem, GetAgendaItemResponse>();
        }
    }
}
