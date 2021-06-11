using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesApi.Controllers
{
    using AutoMapper;

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
    }
}
