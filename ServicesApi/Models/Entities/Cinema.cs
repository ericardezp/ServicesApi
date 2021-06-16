namespace ServicesApi.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using NetTopologySuite.Geometries;

    public class Cinema
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 75)]
        public string CinemaName { get; set; }

        public Point Location { get; set; }

        public List<MovieCinema> MoviesCinemas { get; set; }
    }
}