namespace ServicesApi.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class AddCinemaDto
    {
        [Required]
        [StringLength(maximumLength: 75)]
        public string CinemaName { get; set; }

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

    }
}