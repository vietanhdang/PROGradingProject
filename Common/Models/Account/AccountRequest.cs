using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class AccountRequest
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
