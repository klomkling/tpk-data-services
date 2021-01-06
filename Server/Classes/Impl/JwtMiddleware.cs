using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Classes.Impl
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<AppSettings> _options;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context, IUserService userService, ITokenService tokenService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var principal = tokenService.ValidateToken(token);
                if (principal != null)
                {
                    var user = await userService.GetByUsernameAsync(principal.Identity?.Name);
                    context.Items["User"] = user;
                }
            }

            await _next(context);
        }
    }
}