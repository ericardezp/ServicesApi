namespace ServicesApi
{
    using Microsoft.EntityFrameworkCore;

    using ServicesApi.Models.Entities;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Actor> Actors { get; set; }
    }
}