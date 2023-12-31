﻿namespace Common.Models
{
    public class LoginResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Role { get; set; }
        public string Token { get; set; }
    }
}
