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

    [Route("api/actors")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        private readonly IMapper mapper;

        private readonly IApplicationAzureStorage applicationAzureStorage;

        private readonly string container = "actors";

        public ActorsController(ApplicationDbContext context, IMapper mapper, IApplicationAzureStorage applicationAzureStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.applicationAzureStorage = applicationAzureStorage;
        }

        [HttpGet]
        public async Task<ActionResult<ActorDto>> Get([FromQuery] PaginationDto paginationDto)
        {
            var queryable = this.context.Actors.AsQueryable();
            await this.HttpContext.AddParametersHeader(queryable);
            var actors = queryable.OrderBy(x => x.ActorName).Paginate(paginationDto);
            return this.Ok(this.mapper.Map<List<ActorDto>>(await actors.ToListAsync()));

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDto>> Get(int id)
        {
            var foundActor = await this.context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (foundActor == null)
            {
                return this.NotFound();
            }

            return this.mapper.Map<ActorDto>(foundActor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] AddActorDto addActorDto)
        {
            var actorModel = this.mapper.Map<Actor>(addActorDto);
            if (addActorDto.Photo != null)
            {
               actorModel.Photo = await this.applicationAzureStorage.SaveFile(this.container, addActorDto.Photo);
            }
            this.context.Add(actorModel);
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }

        [HttpPost("searchByName")]
        public async Task<ActionResult<List<MovieActorDto>>> GetByName([FromBody] string actorName)
        {
            if (string.IsNullOrWhiteSpace(actorName))
            {
                return this.Ok(new List<MovieActorDto>());
            }

            return await this.context.Actors.Where(x => x.ActorName.Contains(actorName))
                       .Select(x => new MovieActorDto { Id = x.Id, ActorName = x.ActorName, Photo = x.Photo }).Take(20)
                       .ToListAsync();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] AddActorDto addActorDto)
        {
            var foundActor = await this.context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (foundActor == null)
            {
                return this.NotFound();
            }

            foundActor = this.mapper.Map(addActorDto, foundActor);
            if (addActorDto.Photo != null)
            {
                foundActor.Photo = await this.applicationAzureStorage.EditFile(this.container, addActorDto.Photo, foundActor.Photo);
            }

            await this.context.SaveChangesAsync();
            return this.NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var foundActor = await this.context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (foundActor == null)
            {
                return this.NotFound();
            }

            this.context.Remove(foundActor);
            await this.context.SaveChangesAsync();
            await this.applicationAzureStorage.DeleteFile(foundActor.Photo, this.container);
            return this.NoContent();
        }

    }
}
