using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UserInfo
    {

        public int AccountId { get; set; }
        public string? Password { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int? Role { get; set; }
        public string? Token { get; set; }
    }
}
