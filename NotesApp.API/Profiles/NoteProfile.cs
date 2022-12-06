using AutoMapper;

namespace NotesApp.API.Profiles
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<Entities.Note, Models.NoteDto>();
            CreateMap<Models.NoteForCreationDto, Entities.Note>();
            CreateMap<Entities.Note, Models.NoteForCreationDto>();
        }
    }
}
