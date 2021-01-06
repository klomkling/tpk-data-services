using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}