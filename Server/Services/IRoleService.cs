using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IRoleService
    {
        IEnumerable<string> GetAll();
        Task<IEnumerable<string>> GetAllAsync();
        bool IsAdmin(User user);
        Task<bool> IsAdminAsync(User user);
        bool HasRole(User user, params string[] roles);
        Task<bool> HasRoleAsync(User user, params string[] roles);
    }
}