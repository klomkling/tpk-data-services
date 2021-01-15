using System;
using System.ComponentModel.DataAnnotations;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}