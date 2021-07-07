using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Threading.Tasks;

namespace ServicesApi.Controllers
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using ServicesApi.DTOs;
    using ServicesApi.Models.Entities;

    [Route("api/rating")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;

        private readonly ApplicationDbContext context;

        public RatingsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RatingDto ratingDto)
        {
            var userEmail = this.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            var user = await this.userManager.FindByEmailAsync(userEmail);
            var actualRating =
                await this.context.Ratings.FirstOrDefaultAsync(
                    x => x.MovieId == ratingDto.MovieId && x.UserId == user.Id);
            if (actualRating == null)
            {
                var rating = new Rating { MovieId = ratingDto.MovieId, Score = ratingDto.Score, UserId = user.Id };
                this.context.Add(rating);
            }
            else
            {
                actualRating.Score = ratingDto.Score;
            }

            await this.context.SaveChangesAsync();
            return this.NoContent();
        }
    }
}
