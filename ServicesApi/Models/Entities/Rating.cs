namespace ServicesApi.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;

    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public byte Score { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }
    }
}
