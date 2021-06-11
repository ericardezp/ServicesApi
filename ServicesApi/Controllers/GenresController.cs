namespace ServicesApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using ServicesApi.DTOs;
    using ServicesApi.Models.Entities;
    using ServicesApi.Utilities;

    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<Genre> logger;

        private readonly ApplicationDbContext context;

        private readonly IMapper mapper;

        public GenresController(ILogger<Genre> logger, ApplicationDbContext context, IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDto>>> Get([FromQuery] PaginationDto paginationDto)
        {
            var queryable = this.context.Genres.AsQueryable();
            await this.HttpContext.AddParametersHeader(queryable);
            var genres = queryable.OrderBy(x => x.GenreName).Paginate(paginationDto);
            return this.Ok(this.mapper.Map<List<GenreDto>>(await genres.ToListAsync()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GenreDto>> Get(int id)
        {
            var foundGenre = await this.context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            if (foundGenre == null)
            {
                return this.NotFound();
            }

            return this.mapper.Map<GenreDto>(foundGenre);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AddGenreDto addGenreDto)
        {
            var genreModel = this.mapper.Map<Genre>(addGenreDto);
            this.context.Add(genreModel);
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] AddGenreDto addGenreDto)
        {
            var foundGenre = await this.context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            if (foundGenre == null)
            {
                return this.NotFound();
            }

            foundGenre = this.mapper.Map(addGenreDto, foundGenre);
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existsGenre = await this.context.Genres.AnyAsync(x => x.Id == id);
            if (!existsGenre)
            {
                return this.NotFound();
            }

            this.context.Remove(new Genre { Id = id });
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }
    }
}