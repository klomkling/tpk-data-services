using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class RoleService : IRoleService
    {
        public IEnumerable<string> GetAll()
        {
            return Enumeration.GetAll<TgRoles>().OrderBy(r => r.Value).Select(r => r.DisplayName);
        }

        public async Task<IEnumerable<string>> GetAllAsync()
        {
            return await Task.FromResult(
                Enumeration.GetAll<TgRoles>().OrderBy(r => r.Value).Select(r => r.DisplayName));
        }

        public bool IsAdmin(User user)
        {
            return TgRoles.Administrator.Value.Equals(user.RoleId);
        }

        public async Task<bool> IsAdminAsync(User user)
        {
            return await Task.FromResult(TgRoles.Administrator.Value.Equals(user.RoleId));
        }

        public bool HasRole(User user, params string[] roles)
        {
            var collection = Enumeration.GetAll<TgRoles>()
                .Where(r => roles.Contains(r.DisplayName, StringComparer.InvariantCultureIgnoreCase))
                .Select(r => r.Value);

            return collection.Contains(user.RoleId);
        }

        public async Task<bool> HasRoleAsync(User user, params string[] roles)
        {
            var collection = Enumeration.GetAll<TgRoles>()
                .Where(r => roles.Contains(r.DisplayName, StringComparer.InvariantCultureIgnoreCase))
                .Select(r => r.Value);

            return await Task.FromResult(collection.Contains(user.RoleId));
        }
    }
}