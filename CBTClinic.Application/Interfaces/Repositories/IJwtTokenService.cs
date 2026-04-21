using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    public interface IJwtTokenService
    {
        string GenerateToken(ClaimsIdentity identity, IEnumerable<Claim> additionalClaims = null);
        string GenerateTokenFromUser(string userId, string email, IEnumerable<Claim> additionalClaims = null);
        string GenerateAdminToken(IEnumerable<Claim> claims);
    }
}
