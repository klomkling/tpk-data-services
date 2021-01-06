using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class RegisterRequest
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}