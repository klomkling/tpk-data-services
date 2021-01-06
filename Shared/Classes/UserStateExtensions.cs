using Newtonsoft.Json;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Classes
{
    public static class UserStateExtensions
    {
        public static AuthenticateResponse ToUserState(this string response)
        {
            if (string.IsNullOrEmpty(response)) return null;

            var result = JsonConvert.DeserializeObject<AuthenticateResponse>(response);

            return result;
        }
    }
}