using PreparationTracker.DTO.RequestDTO;
using PreparationTracker.DTO.ResponseDTO;
using PreparationTracker.Model;
using AutoMapper;

namespace PreparationTracker.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Topic mappings
            CreateMap<Topic, TopicRequestDto>().ReverseMap();
            CreateMap<Topic, TopicResponseDto>();

            // Problems mappings
            CreateMap<Problems, ProblemsRequestDto>().ReverseMap();
            CreateMap<Problems, ProblemsResponseDto>();
        }
    }
}
