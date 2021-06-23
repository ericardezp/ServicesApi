namespace ServicesApi.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using ServicesApi.DTOs;

    public class TokenGenerator : ITokenGenerator
    {
        private readonly UserManager<IdentityUser> userManager;

        private readonly IConfiguration configuration;

        public TokenGenerator(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<ResponseAuthentication> GenerateJwt(UserData userData)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, userData.Email) };
            var identityUser = await this.userManager.FindByEmailAsync(userData.Email);
            var claimsDb = await this.userManager.GetClaimsAsync(identityUser);
            claims.AddRange(claimsDb);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.GetValue<string>("JWT_SECRET_KEY")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(30);
            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new ResponseAuthentication
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                ExpirationDate = expires
            };

        }
    }
}