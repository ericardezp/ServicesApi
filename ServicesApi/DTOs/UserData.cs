namespace ServicesApi.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class UserData
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}