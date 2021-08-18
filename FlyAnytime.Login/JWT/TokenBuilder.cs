using FlyAnytime.Core.JWT;
using FlyAnytime.Login.EF;
using FlyAnytime.Login.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Login.JWT
{
    public interface ITokenBuilder
    {
        Task<string> BuildToken(long userId);
    }

    public class TokenBuilder : ITokenBuilder
    {
        private LoginContext _dbContext;
        public TokenBuilder(LoginContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> BuildToken(long userId)
        {
            var user = await _dbContext.Set<User>().FindAsync(userId);

            if (user == null)
                return string.Empty;

            var signingCredentials = new SigningCredentials(JwtOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                //new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
            };
            
            var jwt = new JwtSecurityToken(
                issuer: GetType().FullName,
                claims: claims, 
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials
                );


            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
