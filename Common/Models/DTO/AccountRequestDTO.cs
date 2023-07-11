namespace Common.Models
{
    public class AccountRequestDTO
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Code { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? Role { get; set; } = 3;
    }
}
