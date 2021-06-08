namespace ServicesApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ServicesApi.Models.Entities;

    public class MemoryRepository : IRepository
    {
        private List<Genre> _genres;

        private Guid GuidValue;

        public MemoryRepository()
        {
            this._genres = new List<Genre>
                               {
                                   new() { Id = 1, GenreName = "Drama" },
                                   new() { Id = 2, GenreName = "Acción" },
                                   new() { Id = 3, GenreName = "Comedia" },
                                   new() { Id = 4, GenreName = "Terror" },
                                   new() { Id = 5, GenreName = "Suspenso" }
                               };

            this.GuidValue = Guid.NewGuid();
        }

        public List<Genre> GetCatalogGenres()
        {
            return this._genres;
        }

        public async Task<Genre> GetGenreById(int id)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return this._genres.FirstOrDefault(x => x.Id == id);
        }

        public Guid GetGuidValue()
        {
            return this.GuidValue;
        }

        public void AddGenre(Genre model)
        {
            model.Id = this._genres.Count + 1;
            this._genres.Add(model);
        }
    }
}