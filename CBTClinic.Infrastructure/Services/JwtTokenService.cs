using CBTClinic.Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CBTClinic.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }


        public string GenerateAdminToken(IEnumerable<Claim> claims)
        {
            var list = new List<Claim>(claims ?? new List<Claim>());
            list.Add(new Claim(ClaimTypes.Role, "Admin"));
            return CreateJwt(list);
        }


        public string GenerateToken(ClaimsIdentity identity, IEnumerable<Claim> additionalClaims = null)
        {
            var claims = new List<Claim>();
            if (identity != null)
                claims.AddRange(identity.Claims);

            if (additionalClaims != null)
                claims.AddRange(additionalClaims);

            return CreateJwt(claims);
        }

        public string GenerateTokenFromUser(string userId, string email, IEnumerable<Claim> additionalClaims = null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            if (additionalClaims != null)
                claims.AddRange(additionalClaims);

            return CreateJwt(claims);
        }

        private string CreateJwt(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(
                double.TryParse(_config["JWT:DurationInMinutes"], out var m) ? m : 120
            );

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
