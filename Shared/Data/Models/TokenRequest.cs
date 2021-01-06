using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class TokenRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}