namespace ServicesApi.DTOs
{
    using System.Collections.Generic;

    public class MovieCinemaGenreDto
    {
        public List<GenreDto> Genres { get; set; }

        public List<CinemaDto> Cinemas { get; set; }
    }
}