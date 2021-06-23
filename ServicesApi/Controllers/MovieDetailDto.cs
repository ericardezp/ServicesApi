namespace ServicesApi.Controllers
{
    using System.Collections.Generic;

    using ServicesApi.DTOs;

    public class MovieDetailDto
    {
        public MovieDto Movie { get; set; }

        public List<GenreDto> SelectedGenres { get; set; }

        public List<GenreDto> UnselectedGenres { get; set; }

        public List<CinemaDto> SelectedCinemas { get; set; }

        public List<CinemaDto> UnselectedCinemas { get; set; }

        public List<MovieActorDto> Actors { get; set; }
    }
}
