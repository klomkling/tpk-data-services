using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IUserService : IServiceBase<User>
    {
        User GetByUsername(string username, bool isActive = true);
        Task<User> GetByUsernameAsync(string username, bool isActive = true);
        User GetByEmail(string email, bool isActive = true);
        Task<User> GetByEmailAsync(string email, bool isActive = true);
        User GetByRefreshToken(string refreshToken);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        AuthenticateResponse RefreshToken(TokenRequest model);
        bool RevokeToken();
        IEnumerable<Claim> GetUserClaims(User user, bool isActive = true, params string[] conditions);
        IEnumerable<UserClaim> GetClaimsDictionary(IEnumerable<Claim> claims);

        IEnumerable<UserPermission> GetAllUserPermissionByUserId(int id, bool isActive = true,
            params string[] conditions);

        Task<IEnumerable<UserPermission>> GetAllUserPermissionByUserIdAsync(int id, bool isActive = true,
            params string[] conditions);

        UserPermission InsertUpdateUserPermission(UserPermission model);
        Task<UserPermission> InsertUpdateUserPermissionAsync(UserPermission model);
        int RestoreUserPermission(int id);
        Task<int> RestoreUserPermissionAsync(int id);
        int SoftDeleteUserPermission(int id);
        Task<int> SoftDeleteUserPermissionAsync(int id);
        int BulkDeleteUserPermission(IEnumerable<int> idCollection);
        Task<int> BulkDeleteUserPermissionAsync(IEnumerable<int> idCollection);
        int BulkRestoreUserPermission(IEnumerable<int> idCollection);
        Task<int> BulkRestoreUserPermissionAsync(IEnumerable<int> idCollection);
        int BulkSoftDeleteUserPermission(IEnumerable<int> idCollection);
        Task<int> BulkSoftDeleteUserPermissionAsync(IEnumerable<int> idCollection);
        int DeleteUserPermission(int id);
        Task<int> DeleteUserPermissionAsync(int id);
        bool SoftDeleteUserPermissionByUserId(int id);
        Task<bool> SoftDeleteUserPermissionByUserIdAsync(int id);
        bool RestoreUserPermissionByUserId(int id);
        Task<bool> RestoreUserPermissionByUserIdAsync(int id);
        bool DeleteUserPermissionByUserId(int id);
        Task<bool> DeleteUserPermissionByUserIdAsync(int id);
        bool CloneUserPermission(int fromUserId, int toUserId);
        Task<bool> CloneUserPermissionsAsync(int fromUserId, int toUserId);
    }
}