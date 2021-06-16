namespace ServicesApi.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 200)]
        public string ActorName { get; set; }

        public string Biography { get; set; }

        public DateTime DateBirth { get; set; }

        public string Photo { get; set; }

        public List<MovieActor> MoviesActors { get; set; }
    }
}