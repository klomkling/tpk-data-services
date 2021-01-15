using System.ComponentModel.DataAnnotations;

namespace Tpk.DataServices.Shared.Data.Models
{
    public class UpdatePasswordRequest
    {
        public int UserId { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords not match")]
        public string ConfirmPassword { get; set; }

        public bool IsSuccess { get; set; }
    }
}