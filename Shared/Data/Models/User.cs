using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class User : TgModelBase
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public DateTime? LastVisited { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}