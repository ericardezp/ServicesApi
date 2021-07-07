namespace ServicesApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using ServicesApi.DTOs;
    using ServicesApi.Utilities;

    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;

        private readonly SignInManager<IdentityUser> signInManager;

        private readonly ITokenGenerator tokenGenerator;

        private readonly ApplicationDbContext context;

        private readonly IMapper mapper;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenGenerator tokenGenerator, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenGenerator = tokenGenerator;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdministrator")]
        public async Task<ActionResult<List<UserDto>>> Get([FromQuery] PaginationDto paginationDto)
        {
            var queryable = this.context.Users.AsQueryable();
            await this.HttpContext.AddParametersHeader(queryable);
            var users = await queryable.OrderBy(x => x.Email).Paginate(paginationDto).ToListAsync();
            return this.mapper.Map<List<UserDto>>(users);
        }

        [HttpPost("AddClaim")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdministrator")]
        public async Task<ActionResult> AddClaim([FromBody] string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            await this.userManager.AddClaimAsync(user, new Claim("role", "Administrator"));
            return this.NoContent();
        }

        [HttpPost("RemoveClaim")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdministrator")]
        public async Task<ActionResult> RemoveClaim([FromBody] string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            await this.userManager.RemoveClaimAsync(user, new Claim("role", "Administrator"));
            return this.NoContent();
        }

        [HttpPost("signup")]
        public async Task<ActionResult<ResponseAuthentication>> SignUp([FromBody] UserData userData)
        {
            var identityUser = new IdentityUser { UserName = userData.Email, Email = userData.Email };
            var identityResult = await this.userManager.CreateAsync(identityUser, userData.Password);
            if (identityResult.Succeeded)
            {
                return await this.tokenGenerator.GenerateJwt(userData);
            }

            return this.BadRequest(identityResult.Errors);
        }

        [HttpPost("singin")]
        public async Task<ActionResult<ResponseAuthentication>> SingIn([FromBody] UserData userData)
        {
            var identityUser = await this.userManager.FindByEmailAsync(userData.Email);
            if (identityUser == null)
            {
                return this.NotFound("User not found");
            }
            
            var identityResult = await this.signInManager.PasswordSignInAsync(
                                     userData.Email,
                                     userData.Password,
                                     isPersistent: false,
                                     lockoutOnFailure: false);
            if (identityResult.Succeeded)
            {
                return await this.tokenGenerator.GenerateJwt(userData);
            }

            return this.BadRequest("Email or Password incorrect");
        }
    }
}