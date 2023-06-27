
namespace Common.Models
{
    public class ExamStudentResponse
    {

        public int ExamId { get; set; }


        public int StudentId { get; set; }

        /// <summary>
        /// Start time
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Submit time
        /// </summary>
        public DateTime? SubmitedTime { get; set; }

        /// <summary>
        /// Submit folder
        /// </summary>
        public string? SubmitedFolder { get; set; }

        /// <summary>
        /// Student's status
        /// 0: Not submit
        /// 1: Start
        /// 2: Submit
        /// 3: Submit late
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Student's Log
        /// </summary>
        public string? MarkLog { get; set; }

        /// <summary>
        /// Student's score
        /// </summary>
        public float? Score { get; set; }

        /// <summary>
        /// Count time student submit exam
        /// </summary>
        public int? CountTimeSubmit { get; set; }

        /// <summary>
        /// Exam's name
        /// </summary>
        public string? ExamName { get; set; }

        /// <summary>
        /// Exam's code
        /// </summary>
        public string? ExamCode { get; set; }

        /// <summary>
        /// Exam Start time
        /// </summary>
        public DateTime? ExamStartTime { get; set; }

        /// <summary>
        /// Exam End time
        /// </summary>
        public DateTime? ExamEndTime { get; set; }

        /// <summary>
        /// Exam's status
        /// </summary>
        public int? ExamStatus { get; set; }

        /// <summary>
        /// Exam folder
        /// </summary>
        public string? ExamQuestionFolder { get; set; }
    }


    public class ExamStudentList
    {

        public int ExamId { get; set; }


        public int StudentId { get; set; }

        /// <summary>
        /// Student's status
        /// 0: Not submit
        /// 1: Start
        /// 2: Submit
        /// 3: Submit late
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Exam's name
        /// </summary>
        public string? ExamName { get; set; }

        /// <summary>
        /// Exam's code
        /// </summary>
        public string? ExamCode { get; set; }

        /// <summary>
        /// Exam Start time
        /// </summary>
        public DateTime? ExamStartTime { get; set; }

        /// <summary>
        /// Exam End time
        /// </summary>
        public DateTime? ExamEndTime { get; set; }

        /// <summary>
        /// Exam's status
        /// </summary>
        public int? ExamStatus { get; set; }
    }
}
