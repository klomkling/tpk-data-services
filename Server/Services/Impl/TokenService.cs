using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class TokenService : ITokenService
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<AppSettings> options, ILogger<TokenService> logger)
        {
            _logger = logger;
            _appSettings = options.Value;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims = null)
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var signingCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenOptions = new JwtSecurityToken(
                _appSettings.BaseUrl,
                _appSettings.BaseUrl,
                claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var principal = ValidateToken(token, out var securityToken);
            if (securityToken == null ||
                ((JwtSecurityToken) securityToken).Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature,
                    StringComparison.InvariantCultureIgnoreCase) == false)
                return null;

            return principal;
        }

        public ClaimsPrincipal ValidateToken(string token, out SecurityToken securityToken)
        {
            try
            {
                var tokenValidationParameters = GetTokenValidationParameters();

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                return principal;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error validate token");
                securityToken = null;
                return null;
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            return ValidateToken(token, out _);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret)),
                ValidateLifetime = true
            };
        }
    }
}