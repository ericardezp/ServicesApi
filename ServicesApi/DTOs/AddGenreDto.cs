namespace ServicesApi.DTOs
{
    using System.ComponentModel.DataAnnotations;

    using ServicesApi.Validations;

    public class AddGenreDto
    {
        [Required]
        [StringLength(50)]
        [FirstLetterUppercase]
        public string GenreName { get; set; }
    }
}