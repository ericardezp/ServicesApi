namespace ServicesApi.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 300)]
        public string Title { get; set; }

        public string Resume { get; set; }

        public string Trailer { get; set; }

        public bool MoviesTheaters { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Poster { get; set; }

        public List<MovieActor> MoviesActors { get; set; }

        public List<MovieGenre> MoviesGenres { get; set; }

        public List<MovieCinema> MoviesCinemas { get; set; }
    }
}