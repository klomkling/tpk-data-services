using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Classes.Impl
{
    public static class ClaimPermissionExtensions
    {
        public static int PermissionValue(this UserPermission userPermission)
        {
            var claim = Enumeration.FromValue<TgClaimTypes>(userPermission.ClaimTypeId);
            if (claim == null) return -1;

            var permission = Enumeration.FromValue<TgPermissions>(userPermission.Permission);
            return permission?.Value ?? -1;
        }

        public static Claim ToClaim(this UserPermission userPermission)
        {
            var item = Enumeration.FromValue<TgClaimTypes>(userPermission.ClaimTypeId);
            if (item == null) return null;

            var permission = Enumeration.FromValue<TgPermissions>(userPermission.Permission);
            return permission == null
                ? null
                : new Claim(item.Value.ToString(), permission.Value.ToString());
        }

        public static IEnumerable<Claim> ToClaims(this IEnumerable<UserPermission> collection)
        {
            var result = collection.Select(userPermission => userPermission.ToClaim()).Where(claim => claim != null)
                .ToList();

            return result;
        }
    }
}