namespace Common.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public List<TestCase>? TestCases { get; set; }
    }
}
