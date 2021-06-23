namespace ServicesApi.DTOs
{
    public class MovieFilterDto
    {
        public int CurrentPage { get; set; }

        public int RecordsPerPage { get; set; }

        public PaginationDto PaginationDto => new PaginationDto { CurrentPage = this.CurrentPage, RecordsPerPage = this.RecordsPerPage };

        public string Title { get; set; }

        public int GenreId { get; set; }

        public bool IsMovieShowing { get; set; }

        public bool IsMovieComingSoon { get; set; }
    }
}
