namespace ServicesApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using ServicesApi.Models.Entities;
    
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<Genre> logger;

        public GenresController(ILogger<Genre> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult<List<Genre>> Get()
        {
            return new List<Genre>
                               {
                                   new() { Id = 1, GenreName = "Drama" },
                                   new() { Id = 2, GenreName = "Acción" },
                                   new() { Id = 3, GenreName = "Comedia" },
                                   new() { Id = 4, GenreName = "Terror" },
                                   new() { Id = 5, GenreName = "Suspenso" }
                               };
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre model)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genre model)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}