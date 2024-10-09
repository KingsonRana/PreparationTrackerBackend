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
            CreateMap<TopicRequestDto, Topic>().ReverseMap();
            CreateMap<Topic, TopicResponseDto>().ReverseMap();

            // Problems mappings
            CreateMap<ProblemsRequestDto, Problems>().ReverseMap();
            CreateMap<Problems, ProblemsResponseDto>().ReverseMap();

            CreateMap<ExamRequestDto, Exam>().ReverseMap();
            CreateMap<Exam, ExamResponseDto>().ReverseMap();

            CreateMap<UserSignupRequestDto, User>().ReverseMap();
            CreateMap<User, UserLogInResponseDto>().ReverseMap();
            CreateMap<User, UserDetailDto>().ReverseMap();
            CreateMap<UserUpdateRequestDto,User>().ReverseMap();


        }
    }
}
