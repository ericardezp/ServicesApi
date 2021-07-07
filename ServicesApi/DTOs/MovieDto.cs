namespace ServicesApi.DTOs
{
    using System;
    using System.Collections.Generic;

    public class MovieDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Resume { get; set; }

        public string Trailer { get; set; }

        public bool MoviesTheaters { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Poster { get; set; }

        public byte UserScore { get; set; }

        public double AverageScore { get; set; }

        public List<GenreDto> Genres { get; set; }

        public List<MovieActorDto> Actors { get; set; }

        public List<CinemaDto> Cinemas { get; set; }
    }
}