using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}