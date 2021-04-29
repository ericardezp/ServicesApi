namespace ServicesApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;

    using ServicesApi.Models.Entities;
    using ServicesApi.Repositories;

    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<Genre> logger;

        private readonly IRepository repository;

        public GenresController(ILogger<Genre> logger, IRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult<List<Genre>> Get()
        {
            return this.repository.GetCatalogGenres();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, [FromHeader] string genreName)
        {
            var foundGenre = await this.repository.GetGenreById(id);
            if (foundGenre == null)
            {
                return this.NotFound();
            }

            return this.Ok(foundGenre);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre model)
        {
            return this.NoContent();
        }

        [HttpPut]
        public ActionResult Put()
        {
            return this.NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return this.NoContent();
        }
    }
}