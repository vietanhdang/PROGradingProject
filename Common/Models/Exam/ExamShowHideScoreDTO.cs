namespace PROGradingAPI.Controllers
{
    public class ExamShowHideScoreDTO
    {
        public int ExamId { get; set; }
        public bool IsShowScore { get; set; } = false;
    }
}