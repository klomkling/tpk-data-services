using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Services
{
    public interface IUserService
    {
        bool IsError { get; }
        string ErrorMessage { get; }
        int Id { get; }
        string Username { get; }
        TgRoles Role { get; }
        IEnumerable<TgRoles> Roles { get; }
        bool IsAdministrator { get; }
        bool IsAdminLevel { get; }
        Task<AuthenticateResponse> LoadAsync();
        Task<IEnumerable<UserClaim>> GetClaimsAsync(int userId = 0);
        Task<bool> HasPermissionAsync(TgClaimTypes claimTypes, TgPermissions tpkPermissions);
        Task<bool> HasPermissionOnAsync(TgClaimTypes claimTypes);
        Task<TgPermissions> GetPermissionAsync(TgClaimTypes claimTypes);
        Task<User> GetUserAsync(int userId);
    }
}