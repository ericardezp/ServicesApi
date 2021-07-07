namespace ServicesApi.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Microsoft.AspNetCore.Identity;

    using NetTopologySuite.Geometries;

    using ServicesApi.DTOs;
    using ServicesApi.Models.Entities;

    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            this.CreateMap<Genre, GenreDto>().ReverseMap();

            this.CreateMap<AddGenreDto, Genre>();


            this.CreateMap<Actor, ActorDto>().ReverseMap();

            this.CreateMap<AddActorDto, Actor>().ForMember(x => x.Photo, options => options.Ignore());

            this.CreateMap<AddCinemaDto, Cinema>().ForMember(
                x => x.Location,
                x => x.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude))));
            this.CreateMap<Cinema, CinemaDto>()
                .ForMember(x => x.Latitude, dto => dto.MapFrom(y => y.Location.Y))
                .ForMember(x => x.Longitude, dto => dto.MapFrom(y => y.Location.X));

            this.CreateMap<AddMovieDto, Movie>().ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(this.GetMoviesGenres))
                .ForMember(x => x.MoviesCinemas, options => options.MapFrom(this.GetMoviesCinemas))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(this.GetMoviesActors));
            this.CreateMap<Movie, MovieDto>()
                .ForMember(x => x.Genres, options => options.MapFrom(this.MapperMoviesGenres))
                .ForMember(x => x.Actors, options => options.MapFrom(this.MapperMoviesActors))
                .ForMember(x => x.Cinemas, options => options.MapFrom(this.MapperMoviesCinemas));

            this.CreateMap<IdentityUser, UserDto>();
        }

        private List<CinemaDto> MapperMoviesCinemas(Movie movie, MovieDto movieDto)
        {
            var results = new List<CinemaDto>();
            if (movie.MoviesActors != null)
            {
                foreach (var movieCinemas in movie.MoviesCinemas)
                {
                    results.Add(new CinemaDto
                    {
                        Id = movieCinemas.CinemaId,
                        CinemaName = movieCinemas.Cinema.CinemaName,
                        Latitude = movieCinemas.Cinema.Location.Y,
                        Longitude = movieCinemas.Cinema.Location.X
                    });
                }
            }

            return results;
        }

        private List<MovieActorDto> MapperMoviesActors(Movie movie, MovieDto movieDto)
        {
            var results = new List<MovieActorDto>();
            if (movie.MoviesActors != null)
            {
                foreach (var movieActors in movie.MoviesActors)
                {
                    results.Add(new MovieActorDto
                    {
                        Id = movieActors.ActorId,
                        ActorName = movieActors.Actor.ActorName,
                        Photo = movieActors.Actor.Photo,
                        Order = movieActors.Order,
                        Character = movieActors.Character
                    });
                }
            }

            return results;
        }


        private List<GenreDto> MapperMoviesGenres(Movie movie, MovieDto movieDto)
        {
            var results = new List<GenreDto>();
            if (movie.MoviesGenres != null)
            {
                foreach (var genre in movie.MoviesGenres)
                {
                    results.Add(new GenreDto { Id = genre.GenreId, GenreName = genre.Genre.GenreName });
                }
            }

            return results;
        }

        private List<MovieGenre> GetMoviesGenres(AddMovieDto addMovieDto, Movie movie)
        {
            var results = new List<MovieGenre>();
            if (addMovieDto.GenresIds == null)
            {
                return results;
            }

            results.AddRange(addMovieDto.GenresIds.Select(id => new MovieGenre { GenreId = id }));

            return results;
        }

        private List<MovieCinema> GetMoviesCinemas(AddMovieDto addMovieDto, Movie movie)
        {
            var results = new List<MovieCinema>();
            if (addMovieDto.CinemasIds == null)
            {
                return results;
            }

            results.AddRange(addMovieDto.CinemasIds.Select(id => new MovieCinema { CinemaId = id }));

            return results;
        }

        private List<MovieActor> GetMoviesActors(AddMovieDto addMovieDto, Movie movie)
        {
            var results = new List<MovieActor>();
            if (addMovieDto.Actors == null)
            {
                return results;
            }

            results.AddRange(addMovieDto.Actors.Select(actor => new MovieActor { ActorId = actor.Id, Character = actor.Character }));

            return results;
        }
    }
}