namespace ServicesApi.DTOs
{
    using System.Collections.Generic;

    public class LandingPageDto
    {
        public List<MovieDto> MoviesTheaters { get; set; }

        public List<MovieDto> ComingSoon { get; set; }
}
}