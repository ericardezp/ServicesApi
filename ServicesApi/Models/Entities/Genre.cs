namespace ServicesApi.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ServicesApi.Validations;

    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [FirstLetterUppercase]
        public string GenreName { get; set; }
    }
}