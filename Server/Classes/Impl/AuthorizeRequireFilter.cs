using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Classes.Impl
{
    public class AuthorizeRequiredFilter : IAuthorizationFilter
    {
        private readonly IEnumerable<string> _roles;

        public AuthorizeRequiredFilter(IEnumerable<string> roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user is logged in
            var user = (User) context.HttpContext.Items["User"];
            if (user == null)
            {
                //context.Result = new ForbidResult();
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            // Not required any role
            if (_roles.Any() == false) return;

            // Check role requirement
            var role = Enumeration.FromValue<TgRoles>(user.RoleId);
            if (role == null)
            {
                // context.Result = new UnauthorizedResult();
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            if (_roles.Contains(role.DisplayName, StringComparer.InvariantCultureIgnoreCase) == false)
                // context.Result = new UnauthorizedResult();
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
    }
}