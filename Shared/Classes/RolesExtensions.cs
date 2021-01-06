using System;
using System.Collections.Generic;
using System.Linq;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Classes
{
    public static class RolesExtensions
    {
        public static bool IsAcceptedRoles(this string roleName, params TgRoles[] roles)
        {
            return roles.Any(r => r.DisplayName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsAcceptedRoles(this int roleId, params TgRoles[] roles)
        {
            return roles.Any(r => r.Value == roleId);
        }

        public static IEnumerable<UserRole> ToUserRole(this IEnumerable<TgRoles> source)
        {
            var list = source.Select(role => new UserRole {RoleId = role.Value, RoleName = role.DisplayName}).ToList();
            return list;
        }

        public static bool IsAdminLevel(this User user)
        {
            return user.RoleId == TgRoles.Administrator.Value || user.RoleId == TgRoles.Director.Value;
        }
    }
}