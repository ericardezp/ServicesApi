namespace ServicesApi.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class AddActorDto
    {
        [Required]
        [StringLength(maximumLength: 200)]
        public string ActorName { get; set; }

        public string Biography { get; set; }

        public DateTime DateBirth { get; set; }

        public IFormFile Photo { get; set; }
    }
}