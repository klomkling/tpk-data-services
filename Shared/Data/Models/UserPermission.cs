using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class UserPermission : TgModelBase
    {
        public int UserId { get; set; }
        public int ClaimTypeId { get; set; }
        public int Permission { get; set; }
    }
}