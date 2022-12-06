using AutoMapper;

namespace NotesApp.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.User, Models.UserDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.UserId))
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<Models.UserForCreationDto, Entities.User>();
            CreateMap<Entities.User, Models.UserForCreationDto>();
        }
    }
}
