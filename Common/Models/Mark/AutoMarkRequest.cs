namespace Common.Models.Mark
{
    public class AutoMarkRequest
    {
        public string StudentFolder { get; set; }
        public string TestCaseFolder { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
    }
}
