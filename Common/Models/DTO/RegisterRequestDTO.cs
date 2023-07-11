

using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class RegisterRequestDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^\S+@(fpt\.edu\.vn|fe\.edu\.vn)$", ErrorMessage = "Email must be a valid @fpt.edu.vn or @fe.edu.vn address.")]
        public string Email { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Code { get; set; }

        public string Phone { get; set; }
        public string? Address { get; set; }
        public int? Role { get; set; } = 3;

        public bool ToLogin { get; set; } = false;
    }
}
