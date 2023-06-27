namespace Common.Models
{
    public class TestCase
    {
        public int TestCaseId { get; set; }
        public string? InputType { get; set; }
        public string? InputValue { get; set; }
        public string? ExpectedType { get; set; }
        public string? Expected { get; set; }
        public float Score { get; set; }
    }
}
