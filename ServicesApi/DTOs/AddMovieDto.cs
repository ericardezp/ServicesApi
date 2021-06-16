namespace ServicesApi.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using ServicesApi.Utilities;

    public class AddMovieDto
    {
        [Required]
        [StringLength(maximumLength: 300)]
        public string Title { get; set; }

        public string Resume { get; set; }

        public string Trailer { get; set; }

        public bool MoviesTheaters { get; set; }

        public DateTime ReleaseDate { get; set; }

        public IFormFile Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeCustomBinder<List<int>>))]
        public List<int> GenresIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeCustomBinder<List<int>>))]
        public List<int> CinemasIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeCustomBinder<List<AddMovieActorDto>>))]
        public List<AddMovieActorDto> Actors { get; set; }
    }
}