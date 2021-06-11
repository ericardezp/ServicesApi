namespace ServicesApi.DTOs
{
    using System;

    public class ActorDto
    {
        public int Id { get; set; }

        public string ActorName { get; set; }

        public string Biography { get; set; }

        public DateTime DateBirth { get; set; }

        public string Photo { get; set; }

    }
}