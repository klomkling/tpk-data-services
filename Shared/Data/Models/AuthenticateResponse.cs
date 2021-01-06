using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }
        public int RoleId { get; set; }
        public string RefreshToken { get; set; }
    }
}