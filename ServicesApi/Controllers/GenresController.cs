namespace ServicesApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using ServicesApi.Models.Entities;
    
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<Genre> logger;

        private readonly ApplicationDbContext context;

        public GenresController(ILogger<Genre> logger, ApplicationDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            var genres = await this.context.Genres.ToListAsync();
            return this.Ok(genres);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Genre model)
        {
            this.context.Add(model);
            await this.context.SaveChangesAsync();
            return this.NoContent();
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