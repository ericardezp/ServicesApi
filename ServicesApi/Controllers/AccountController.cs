using Microsoft.AspNetCore.Mvc;

namespace ServicesApi.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    using ServicesApi.DTOs;
    using ServicesApi.Utilities;

    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;

        private readonly SignInManager<IdentityUser> signInManager;

        private readonly ITokenGenerator tokenGenerator;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenGenerator tokenGenerator)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenGenerator = tokenGenerator;
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