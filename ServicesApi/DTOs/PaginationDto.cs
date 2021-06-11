namespace ServicesApi.DTOs
{
    public class PaginationDto
    {
        private int recordsPerPage = 10;

        private readonly int maxRecordsPerPage = 50;

        public int CurrentPage { get; set; } = 1;

        public int RecordsPerPage
        {
            get => this.recordsPerPage;
            set => this.recordsPerPage = (value > this.maxRecordsPerPage) ? this.maxRecordsPerPage : value;
        }
    }
}