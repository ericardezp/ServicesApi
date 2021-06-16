namespace ServicesApi.Models.Entities
{
    public class MovieCinema
    {
        public int MovieId { get; set; }

        public int CinemaId { get; set; }

        public Movie Movie { get; set; }

        public Cinema Cinema { get; set; }
    }
}