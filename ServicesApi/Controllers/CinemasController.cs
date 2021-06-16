namespace ServicesApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using ServicesApi.DTOs;
    using ServicesApi.Models.Entities;
    using ServicesApi.Utilities;

    [Route("api/cinemas")]
    [ApiController]
    public class CinemasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        private readonly IMapper mapper;

        public CinemasController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CinemaDto>>> Get([FromQuery] PaginationDto paginationDto)
        {
            var queryable = this.context.Cinemas.AsQueryable();
            await this.HttpContext.AddParametersHeader(queryable);
            var cinemas = queryable.OrderBy(x => x.CinemaName).Paginate(paginationDto);
            return this.Ok(this.mapper.Map<List<CinemaDto>>(await cinemas.ToListAsync()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CinemaDto>> Get(int id)
        {
            var foundCinema = await this.context.Cinemas.FirstOrDefaultAsync(x => x.Id == id);
            if (foundCinema == null)
            {
                return this.NotFound();
            }

            return this.Ok(this.mapper.Map<CinemaDto>(foundCinema));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AddCinemaDto addCinemaDto)
        {
            var cine = this.mapper.Map<Cinema>(addCinemaDto);
            this.context.Add(cine);
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] AddCinemaDto addCinemaDto)
        {
            var foundCinema = await this.context.Cinemas.FirstOrDefaultAsync(x => x.Id == id);
            if (foundCinema == null)
            {
                return this.NotFound();
            }

            foundCinema = this.mapper.Map(addCinemaDto, foundCinema);
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existsCinema = await this.context.Cinemas.AnyAsync(x => x.Id == id);
            if (!existsCinema)
            {
                return this.NotFound();
            }

            this.context.Remove(new Cinema { Id = id });
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }
    }
}
