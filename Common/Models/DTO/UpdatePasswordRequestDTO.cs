using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class UpdatePasswordRequestDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string NewPassword { get; set; }
    }
}
