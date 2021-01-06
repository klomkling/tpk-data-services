using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Services
{
    public interface IAuthService
    {
        Task<AuthenticateResponse> Login(AuthenticateRequest model);
        Task<bool> Logout();
        Task<bool> RefreshToken();
    }
}