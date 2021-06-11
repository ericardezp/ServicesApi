namespace ServicesApi.Utilities
{
    using AutoMapper;

    using ServicesApi.DTOs;
    using ServicesApi.Models.Entities;

    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            this.CreateMap<Genre, GenreDto>().ReverseMap();
            this.CreateMap<AddGenreDto, Genre>();
            this.CreateMap<Actor, ActorDto>().ReverseMap();
            this.CreateMap<AddActorDto, Actor>()
                .ForMember(x => x.Photo, options => options.Ignore());
        }
    }
}