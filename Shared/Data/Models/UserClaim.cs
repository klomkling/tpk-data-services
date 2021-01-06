using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class UserClaim
    {
        public int ClaimId { get; set; }
        public int Permission { get; set; }
    }
}