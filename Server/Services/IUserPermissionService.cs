using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IUserPermissionService : IServiceBase<UserPermission>
    {
        IEnumerable<Claim> GetClaims(int userId, bool isActive = true, params string[] conditions);
        Task<IEnumerable<Claim>> GetClaimsAsync(int userId, bool isActive = true, params string[] conditions);
        IEnumerable<UserPermission> GetByUserId(int id, bool isActive = true, params string[] conditions);
        Task<IEnumerable<UserPermission>> GetByUserIdAsync(int id, bool isActive = true, params string[] conditions);
        UserPermission GetByRestrictObjectId(int id, bool isActive = true);
        bool SoftDeleteByUserId(int id);
        Task<bool> SoftDeleteByUserIdAsync(int id);
        bool RestoreByUserId(int id);
        Task<bool> RestoreByUserIdAsync(int id);
        bool DeleteByUserId(int id);
        Task<bool> DeleteByUserIdAsync(int id);
        bool ClonePermissions(int fromUserId, int toUserId);
        Task<bool> ClonePermissionsAsync(int fromUserId, int toUserId);
    }
}