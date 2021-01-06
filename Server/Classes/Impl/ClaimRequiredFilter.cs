using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Constants;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Classes.Impl
{
    public class ClaimRequiredFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;
        private readonly IRoleService _roleService;
        private readonly IUserPermissionService _userPermissionService;

        public ClaimRequiredFilter(Claim claim, IRoleService roleService, IUserPermissionService userPermissionService)
        {
            _claim = claim;
            _roleService = roleService;
            _userPermissionService = userPermissionService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get user
            var user = (User) context.HttpContext.Items["User"];
            if (user == null)
            {
                // context.Result = new ForbidResult();
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            // Check if has role Administrator or Director
            if (_roleService.HasRole(user, RestrictRoles.Administrator, RestrictRoles.Director)) return;

            if (int.TryParse(_claim.Type, out var restrictObjectId) == false)
            {
                // context.Result = new UnauthorizedResult();
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            // Get user permission
            var userPermission = _userPermissionService.GetByRestrictObjectId(restrictObjectId);
            if (_userPermissionService.IsError)
            {
                // context.Result = new UnauthorizedResult();
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            // Check permission
            if (int.TryParse(_claim.Value, out var minimum) == false) minimum = 0;

            if (userPermission.Permission >= minimum) return;

            // context.Result = new UnauthorizedResult();
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
    }
}