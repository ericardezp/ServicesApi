using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesApi.Controllers
{
    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using ServicesApi.DTOs;
    using ServicesApi.Models.Entities;
    using ServicesApi.Utilities;

    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        private readonly IMapper mapper;

        private readonly IApplicationAzureStorage applicationAzureStorage;

        private readonly string container = "actors";


        public MoviesController(ApplicationDbContext context, IMapper mapper, IApplicationAzureStorage applicationAzureStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.applicationAzureStorage = applicationAzureStorage;
        }

        [HttpGet]
        public async Task<ActionResult<LandingPageDto>> Get()
        {
            var top = 5;
            var today = DateTime.Today;

            var comingSoon = await this.context.Movies.Where(x => x.ReleaseDate > today).OrderBy(x => x.ReleaseDate)
                                 .Take(top).ToListAsync();
            var moviesTheaters = await this.context.Movies.Where(x => x.MoviesTheaters).OrderBy(x => x.ReleaseDate)
                                     .Take(top).ToListAsync();

            var landingPageDto = new LandingPageDto();
            landingPageDto.ComingSoon = this.mapper.Map<List<MovieDto>>(comingSoon);
            landingPageDto.MoviesTheaters = this.mapper.Map<List<MovieDto>>(moviesTheaters);
            return this.Ok(landingPageDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieDto>> Get(int id)
        {
            var movie = await this.context.Movies.Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor).Include(x => x.MoviesCinemas)
                .ThenInclude(x => x.Cinema).FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return this.NotFound();
            }

            var movieDto = this.mapper.Map<MovieDto>(movie);
            movieDto.Actors = movieDto.Actors.OrderBy(x => x.Order).ToList();
            return this.Ok(movieDto);
        }

        [HttpGet("catalogs")]
        public async Task<ActionResult<MovieCinemaGenreDto>> GetCatalogs()
        {
            var cinemas = await this.context.Cinemas.ToListAsync();
            var genres = await this.context.Genres.ToListAsync();
            var cinemasDto = this.mapper.Map<List<CinemaDto>>(cinemas);
            var genresDto = this.mapper.Map<List<GenreDto>>(genres);

            return this.Ok(new MovieCinemaGenreDto { Cinemas = cinemasDto, Genres = genresDto });
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] AddMovieDto addMovieDto)
        {
            var movie = this.mapper.Map<Movie>(addMovieDto);
            if (addMovieDto.Poster != null)
            {
                movie.Poster = await this.applicationAzureStorage.SaveFile(this.container, addMovieDto.Poster);
            }

            SetOrderActor(movie);
            this.context.Add(movie);
            await this.context.SaveChangesAsync();
            return this.NoContent();

        }

        private static void SetOrderActor(Movie movie)
        {
            if (movie.MoviesActors == null)
            {
                return;
            }

            for (var i = 0; i < movie.MoviesActors.Count; i++)
            {
                movie.MoviesActors[i].Order = i;
            }
        }
    }
}
