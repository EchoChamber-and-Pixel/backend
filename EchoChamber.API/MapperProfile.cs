using AutoMapper;
using EchoChamber.API.DTO;
using EchoChamber.API.Models;

namespace EchoChamber.API
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Record, RecordView>()
                .ForMember(r => r.ModeId, opt => opt.MapFrom(r => r.Mode));
        }
    }
}