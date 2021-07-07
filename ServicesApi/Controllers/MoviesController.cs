namespace ServicesApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using ServicesApi.DTOs;
    using ServicesApi.Models.Entities;
    using ServicesApi.Utilities;

    [Route("api/movies")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdministrator")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        private readonly IMapper mapper;

        private readonly IApplicationAzureStorage applicationAzureStorage;

        private readonly UserManager<IdentityUser> userManager;

        private readonly string container = "actors";


        public MoviesController(ApplicationDbContext context, IMapper mapper, IApplicationAzureStorage applicationAzureStorage, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.applicationAzureStorage = applicationAzureStorage;
            this.userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<LandingPageDto>> Get()
        {
            var top = 5;
            var today = DateTime.Today;

            var comingSoon = await this.context.Movies.Where(x => x.ReleaseDate > today).OrderBy(x => x.ReleaseDate)
                                 .Take(top).ToListAsync();
            var moviesTheaters = await this.context.Movies.Where(x => x.MoviesTheaters).OrderBy(x => x.ReleaseDate)
                                     .Take(top).ToListAsync();

            var landingPageDto = new LandingPageDto
            {
                ComingSoon = this.mapper.Map<List<MovieDto>>(comingSoon),
                MoviesTheaters = this.mapper.Map<List<MovieDto>>(moviesTheaters)
            };
            return landingPageDto;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDto>> Get(int id)
        {
            var movie = await this.context.Movies.Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor).Include(x => x.MoviesCinemas)
                .ThenInclude(x => x.Cinema).FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return this.NotFound();
            }

            var averageScore = 0.0;
            byte userScore = 0;
            if (await this.context.Ratings.AnyAsync(x => x.MovieId == id) && this.HttpContext.User.Identity.IsAuthenticated)
            {
                averageScore = await this.context.Ratings.Where(x => x.MovieId == id).AverageAsync(x => x.Score);
                var userEmail = this.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
                var user = await this.userManager.FindByEmailAsync(userEmail);
                var scoreDb = await this.context.Ratings.FirstOrDefaultAsync(x => x.UserId == user.Id && x.MovieId == id);
                if (scoreDb != null)
                {
                    userScore = scoreDb.Score;
                }
            }

            var movieDto = this.mapper.Map<MovieDto>(movie);
            movieDto.UserScore = userScore;
            movieDto.AverageScore = averageScore;
            movieDto.Actors = movieDto.Actors.OrderBy(x => x.Order).ToList();
            return movieDto;
        }

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MovieDto>>> Filter([FromQuery] MovieFilterDto movieFilterDto)
        {
            var moviesQueryable = this.context.Movies.AsQueryable();
            if (!string.IsNullOrEmpty(movieFilterDto.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(movieFilterDto.Title));
            }

            if (movieFilterDto.MoviesTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesTheaters);
            }

            if (movieFilterDto.ComingSoon)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (movieFilterDto.GenreId != 0)
            {
                moviesQueryable = moviesQueryable.Where(
                    x => x.MoviesGenres.Select(y => y.GenreId).Contains(movieFilterDto.GenreId));
            }

            await this.HttpContext.AddParametersHeader(moviesQueryable);
            var movies = await moviesQueryable.Paginate(movieFilterDto.PaginationDto).ToListAsync();
            return this.mapper.Map<List<MovieDto>>(movies);
        }

        [HttpGet("MovieDetail/{id:int}")]
        public async Task<ActionResult<MovieDetailDto>> GetMovieDetail(int id)
        {
            var foundMovie = await this.Get(id);
            if (foundMovie.Result is NotFoundResult)
            {
                return this.NotFound();
            }

            var movie = foundMovie.Value;
            var selectedGenresIds = movie.Genres.Select(x => x.Id).ToList();
            var unselectedGenresIds =
                await this.context.Genres.Where(x => !selectedGenresIds.Contains(x.Id)).ToListAsync();
            var selectedCinemasIds = movie.Cinemas.Select(x => x.Id).ToList();
            var unselectedCinemasIds =
                await this.context.Cinemas.Where(x => !selectedCinemasIds.Contains(x.Id)).ToListAsync();

            var unselectedGenresDto = this.mapper.Map<List<GenreDto>>(unselectedGenresIds);
            var unselectedCinemasDto = this.mapper.Map<List<CinemaDto>>(unselectedCinemasIds);

            return new MovieDetailDto
            {
                Movie = movie,
                SelectedGenres = movie.Genres,
                UnselectedGenres = unselectedGenresDto,
                SelectedCinemas = movie.Cinemas,
                UnselectedCinemas = unselectedCinemasDto,
                Actors = movie.Actors
            };
        }

        [HttpGet("catalogs")]
        public async Task<ActionResult<MovieCinemaGenreDto>> GetCatalogs()
        {
            var cinemas = await this.context.Cinemas.ToListAsync();
            var genres = await this.context.Genres.ToListAsync();
            var cinemasDto = this.mapper.Map<List<CinemaDto>>(cinemas);
            var genresDto = this.mapper.Map<List<GenreDto>>(genres);

            return new MovieCinemaGenreDto { Cinemas = cinemasDto, Genres = genresDto };
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] AddMovieDto addMovieDto)
        {
            var movie = this.mapper.Map<Movie>(addMovieDto);
            if (addMovieDto.Poster != null)
            {
                movie.Poster = await this.applicationAzureStorage.SaveFile(this.container, addMovieDto.Poster);
            }

            SetOrderActor(movie);
            this.context.Add(movie);
            await this.context.SaveChangesAsync();
            return movie.Id;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] AddMovieDto addMovieDto)
        {
            var movie = await this.context.Movies
                            .Include(x => x.MoviesActors)
                            .Include(x => x.MoviesGenres)
                            .Include(x => x.MoviesCinemas)
                            .FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return this.NotFound();
            }

            movie = this.mapper.Map(addMovieDto, movie);
            if (addMovieDto.Poster != null)
            {
                movie.Poster = await this.applicationAzureStorage.EditFile(
                                   this.container,
                                   addMovieDto.Poster,
                                   movie.Poster);
            }

            SetOrderActor(movie);
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var foundMovie = await this.context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (foundMovie == null)
            {
                return this.NotFound();
            }

            this.context.Remove(foundMovie);
            await this.context.SaveChangesAsync();
            await this.applicationAzureStorage.DeleteFile(foundMovie.Poster, this.container);
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
