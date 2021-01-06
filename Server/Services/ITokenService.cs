using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Tpk.DataServices.Server.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims = null);
        string GenerateRefreshToken();
        ClaimsPrincipal ValidateToken(string token);
        ClaimsPrincipal ValidateToken(string token, out SecurityToken securityToken);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}