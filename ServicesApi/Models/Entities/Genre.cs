namespace ServicesApi.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ServicesApi.Validations;

    public class Genre : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        // [FirstLetterUppercase]
        public string GenreName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(this.GenreName))
            {
                var firstLetter = this.GenreName[0].ToString();
                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult(
                        "La primer letra debe ser mayúscula", 
                        new[] { nameof(this.GenreName) });
                }
            }
        }
    }
}