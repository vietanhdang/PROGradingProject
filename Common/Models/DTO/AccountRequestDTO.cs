using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class AccountRequestDTO
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Code { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }
        public int? Role { get; set; } = 3;
    }
}
