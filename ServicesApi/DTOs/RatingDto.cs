namespace ServicesApi.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class RatingDto
    {
        public int MovieId { get; set; }

        [Range(1, 5)]
        public byte Score { get; set; }
    }
}
