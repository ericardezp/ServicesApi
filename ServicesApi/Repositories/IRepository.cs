namespace ServicesApi.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ServicesApi.Models.Entities;

    public interface IRepository
    {
        List<Genre> GetCatalogGenres();

        Task<Genre> GetGenreById(int id);
    }
}